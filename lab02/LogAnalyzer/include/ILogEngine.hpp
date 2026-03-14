#pragma once
#include <string>

enum class LogLevel{ Info, Warning, Error, Critical};

class ILogEngine{
    public:
        virtual void addLogLine(const std::string& message, LogLevel level) = 0;
        virtual size_t getLineCount() const = 0;
        virtual std::string getLineMessage(size_t index) const = 0;
        virtual LogLevel getLineLevel(size_t index) const = 0;
        virtual size_t getMemoryUsage() const = 0;
        virtual size_t getObjectCount() const = 0;
        virtual ~ILogEngine() = default;
};