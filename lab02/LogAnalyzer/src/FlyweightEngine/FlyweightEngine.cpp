#include "FlyweightEngine.hpp"

void FlyWeightEngine::addLogLine(const std::string& message, LogLevel level){
    if (sharedStyles.find(level) == sharedStyles.end()){
        LogStyle newStyle;
        newStyle.fontName = "Consolas";
        newStyle.fontSize = 12;

        if (level == LogLevel::Info){
            newStyle.colorR = 255;
            newStyle.colorG = 255;
            newStyle.colorB = 255;
            newStyle.isBold = false;
            newStyle.isItalic = false;
        }

        else if (level == LogLevel::Warning){
            newStyle.colorR = 255;
            newStyle.colorG = 255;
            newStyle.colorB = 0;
            newStyle.isBold = false;
            newStyle.isItalic = true;
        }

        else if (level == LogLevel::Error){
            newStyle.colorR = 255;
            newStyle.colorG = 0;
            newStyle.colorB = 0;
            newStyle.isBold = true;
            newStyle.isItalic = false;
        }

        else if (level == LogLevel::Critical){
            newStyle.colorR = 255;
            newStyle.colorG = 0;
            newStyle.colorB = 0;
            newStyle.isBold = true;
            newStyle.isItalic = true;
        }
        
        sharedStyles[level] = newStyle;
    }
    FlyWeightEntry entry;
    entry.message = message;
    entry.level = level;
    entry.style = &sharedStyles[level];
    logs.push_back(entry);
}

size_t FlyWeightEngine::getLineCount() const{
    return logs.size();
}

std::string FlyWeightEngine::getLineMessage(size_t index) const{
    return logs[index].message;
}

LogLevel FlyWeightEngine::getLineLevel(size_t index) const{
    return logs[index].level;
}

size_t FlyWeightEngine::getMemoryUsage() const{
    size_t total = logs.size() * sizeof(FlyWeightEntry);

    for (const auto& entry: logs){
        total += entry.message.size();
    }
    total += sharedStyles.size() * sizeof(LogStyle);

    for (auto const& [level, style]: sharedStyles){
        total += style.fontName.size();
    }

    return total;
}

size_t FlyWeightEngine::getObjectCount() const{
    return sharedStyles.size();
}