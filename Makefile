.PHONY: all clean

CC=fsharpc --nologo

LEX						= lib/FsLexYacc/src/FsLex/bin/Release/netcoreapp3.1/fslex
YACC					 = lib/FsLexYacc/src/FsYacc/bin/Release/netcoreapp3.1/fsyacc
LEXYACCRUNTIME = lib/FsLexYacc/src/FsLex/bin/Release/netcoreapp3.1/FsLexYacc.Runtime.dll
LEXYACC				= $(LEX) $(YACC)

OUT=bin/lpt

LexerGen=bin/Lexer.fs
ParserGen=bin/Parser.fs
AbSynLib=bin/AbSyn.dll
ParserLib=bin/Parser.dll
LexerLib=bin/Lexer.dll

.PHONY: bin clean

all: $(LEXYACC) bin $(OUT)

bin:
	@mkdir -p bin

$(OUT): src/lpt.fsx $(ParserLib) $(LexerLib) $(AbSynLib)
	cp $(LEXYACCRUNTIME) bin
	LD_LIBRARY_PATH=bin $(CC) src/lpt.fsx -r $(AbSynLib) --staticlink:AbSyn -r $(ParserLib) --staticlink:Parser -r $(LexerLib) --staticlink:Lexer -r bin/FsLexYacc.Runtime.dll --staticlink:FsLexYacc.Runtime -o $(OUT)
	./$(OUT) "~p"

$(LexerGen): src/Lexer.fsl
	$(LEX) src/Lexer.fsl --unicode -o $(LexerGen)

$(ParserGen): src/Parser.fsp
	$(YACC) -v --module Parser src/Parser.fsp -o $(ParserGen)

$(AbSynLib): src/AbSyn.fs
	$(CC) -a src/AbSyn.fs -o $(AbSynLib)

$(ParserLib): $(ParserGen) $(AbSynLib)
	$(CC) -a $(ParserGen) -r $(AbSynLib) -r $(LEXYACCRUNTIME) -o $(ParserLib)

$(LexerLib): $(LexerGen) $(AbSynLib) $(ParserLib)
	$(CC) -a $(LexerGen) -r $(AbSynLib) -r $(ParserLib) -r $(LEXYACCRUNTIME) -o $(LexerLib)

$(LEXYACC):
	(cd lib/FsLexYacc && ./build.sh --target release)

clean:
	rm bin/*
