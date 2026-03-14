#include <map>
#include <string>
#include <vector>
#include "ILogEngine.hpp"

struct LogStyle{
    std::string fontName;
    int fontSize;
    int colorR;
    int colorG;
    int colorB;
    bool isBold;
    bool isItalic;
};

struct FlyWeightEntry{
    std::string message;
    LogLevel level;
    const LogStyle* style;
};

class FlyWeightEngine: public ILogEngine{
    public:
        FlyWeightEngine() = default;
        ~FlyWeightEngine() override = default;
        void addLogLine(const std::string& message, LogLevel level) override;
        size_t getLineCount() const override;
        std::string getLineMessage(size_t index) const override;
        LogLevel getLineLevel(size_t index) const override;
        size_t getMemoryUsage() const override;
        size_t getObjectCount() const override;
    
    private:
        std::vector<FlyWeightEntry> logs;
        std::map<LogLevel, LogStyle> sharedStyles;
};