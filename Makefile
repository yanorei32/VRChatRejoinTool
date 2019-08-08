CSC		= /cygdrive/c/windows/microsoft.net/framework/v4.0.30319/csc.exe
TARGET	= VRChatRejoin.exe
SRC		= main.cs

DEPS	=

CSC_FLAGS		=	/nologo /target:winexe \
					/win32icon:res\\icon.ico \
					/resource:res\\icon.ico,icon \
					/resource:res\\logo.png,logo \
					/utf8output
DEBUG_FLAGS		= 
RELEASE_FLAGS	= 

.PHONY: debug
debug: CSC_FLAGS+=$(DEBUG_FLAGS)
debug: all

.PHONY: release
release: CSC_FLAGS+=$(RELEASE_FLAGS)
release: all

.PHONY: genzip
genzip:
	zip -r VRChatRejoinTool.zip VRChatRejoinTool

all: $(TARGET)
$(TARGET): $(SRC) $(DEPS)
	$(CSC) $(CSC_FLAGS) /out:$(TARGET) $(SRC)

.PHONY: clean
clean:
	rm $(TARGET)


