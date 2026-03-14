#pragma once
#include <string>
#include <vector>
#include "ILogEngine.hpp"

struct LogEntry{
    std::string message;
    LogLevel level;
    std::string fontName;
    int fontSize;
    int colorR;
    int colorG;
    int colorB;
    bool isBold;
    bool isItalic;
};

class NaiveEngine: public ILogEngine{
    public:
        NaiveEngine() = default;
        ~NaiveEngine() override = default;
        void addLogLine(const std::string& message, LogLevel level) override;
        size_t getLineCount() const override;
        std::string getLineMessage(size_t index) const override;
        LogLevel getLineLevel(size_t index) const override;
        size_t getMemoryUsage() const override;
        size_t getObjectCount() const override;

    private:
        std::vector<LogEntry> logs;
};