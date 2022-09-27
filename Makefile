PREFIX		= $(shell if [ "`uname | tr -d 0-9`" = "CYGWIN_NT-.-" ]; then echo cygdrive; else echo mnt; fi)
CSC			= /$(PREFIX)/c/windows/microsoft.net/framework/v4.0.30319/csc.exe
PROJ_NAME	= VRChatRejoinTool
TARGET_DIR	= bin/csc
TARGET_DIR_	= $(shell echo $(TARGET_DIR) | sed -e 's/\//\\\\/g')
TARGET		= $(TARGET_DIR_)\\$(PROJ_NAME).exe

SRC			=	$(shell echo ./src/**/*.cs ./src/*.cs)

SRC_		=	$(shell echo $(SRC) | sed -e 's/\//\\\\/g')

DEPS		=	res/icon.ico \
				res/logo.png

CSC_FLAGS		=	/nologo \
					/target:winexe \
					/win32icon:res\\icon.ico \
					/resource:res\\icon.ico,icon \
					/resource:res\\logo.png,logo \
					/utf8output \
					/optimize

DEBUG_FLAGS		= 
RELEASE_FLAGS	=

.PHONY: release
release: CSC_FLAGS+=$(RELEASE_FLAGS)
release: all

$(PROJ_NAME).zip: all
	zip -r \
		$(PROJ_NAME).zip \
		$(PROJ_NAME)

$(PROJ_NAME)-malware-analysis.zip: all
	zip -r \
		-P infected \
		$(PROJ_NAME)-malware-analysis.zip \
		$(PROJ_NAME)

.PHONY: genzip
genzip: $(PROJ_NAME).zip

.PHONY: genzip-for-malware-analysis
genzip-for-malware-analysis: $(PROJ_NAME)-malware-analysis.zip

all: $(TARGET)
$(TARGET): $(SRC) $(DEPS)
	-mkdir -p $(TARGET_DIR)
	$(CSC) $(CSC_FLAGS) /out:$(TARGET) $(SRC_)

.PHONY: clean
clean:
	-rm $(TARGET)
	-rm $(PROJ_NAME).zip
	-rm $(PROJ_NAME)-malware-analysis.zip

