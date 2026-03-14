#include "NaiveEngine.hpp"

void NaiveEngine::addLogLine(const std::string& message, LogLevel level){
    LogEntry entry;
    entry.message = message;
    entry.level = level;

    entry.fontName = "Consolas";
    entry.fontSize = 12;

    if (level == LogLevel::Info){
        entry.colorR = 255;
        entry.colorG = 255;
        entry.colorB = 255;
        entry.isBold = false;
        entry.isItalic = false;
    }

    else if (level == LogLevel::Warning){
        entry.colorR = 255;
        entry.colorG = 255;
        entry.colorB = 0;
        entry.isBold = false;
        entry.isItalic = true;
    }

    else if (level == LogLevel::Error){
        entry.colorR = 255;
        entry.colorG = 0;
        entry.colorB = 0;
        entry.isBold = true;
        entry.isItalic = false;
    }

    else if (level == LogLevel::Critical){
        entry.colorR = 255;
        entry.colorG = 0;
        entry.colorB = 0;
        entry.isBold = true;
        entry.isItalic = false;
    }

    logs.push_back(entry);
}

size_t NaiveEngine::getLineCount() const{
    return logs.size();
}

std::string NaiveEngine::getLineMessage(size_t index) const{
    return logs[index].message;
}

LogLevel NaiveEngine::getLineLevel(size_t index) const{
    return logs[index].level;
}

size_t NaiveEngine::getMemoryUsage() const{
    size_t total = logs.size() * sizeof(LogEntry);
    
    for (const auto& entry: logs){
        total += entry.message.size();
        total += entry.fontName.size();
    }

    return total;
}

size_t NaiveEngine::getObjectCount() const{
    return logs.size();
}