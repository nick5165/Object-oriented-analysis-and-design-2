#include "imgui.h"
#include "imgui_impl_glfw.h"
#include "imgui_impl_opengl3.h"
#include <GLFW/glfw3.h> 

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string>
#include <vector>

// Подключаем твои движки
#include "ILogEngine.hpp"
#include "NaiveEngine/NaiveEngine.hpp"
#include "FlyweightEngine/FlyweightEngine.hpp"

int main() {
    // 1. Инициализация GLFW
    if (!glfwInit()) return 1;

    const char* glsl_version = "#version 130";
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);

    // Создание окна
    GLFWwindow* window = glfwCreateWindow(1280, 720, "Log Analyzer Professional", NULL, NULL);
    if (window == NULL) return 1;
    glfwMakeContextCurrent(window);
    glfwSwapInterval(1); 

    // 2. Инициализация ImGui
    IMGUI_CHECKVERSION();
    ImGui::CreateContext();
    ImGuiIO& io = ImGui::GetIO(); (void)io;
    ImGui::StyleColorsDark();

    ImGui_ImplGlfw_InitForOpenGL(window, true);
    ImGui_ImplOpenGL3_Init(glsl_version);

    // 3. Состояние приложения
    srand((unsigned int)time(NULL));
    NaiveEngine naive;
    FlyWeightEngine flyweight;
    ILogEngine* currentEngine = &naive;

    bool systemRunning = false;
    float logTimer = 0.0f;
    float logSpeed = 0.05f;
    std::vector<float> memHistory;
    float maxMem = 0.1f;

    // 4. Главный цикл
    while (!glfwWindowShouldClose(window)) {
        glfwPollEvents();

        ImGui_ImplOpenGL3_NewFrame();
        ImGui_ImplGlfw_NewFrame();
        ImGui::NewFrame();

        // Логика генерации
        if (systemRunning) {
            logTimer += io.DeltaTime;
            if (logTimer > logSpeed) {
                logTimer = 0.0f;
                int r = rand() % 100;
                if (r < 60) currentEngine->addLogLine("DB_SERVER: Connection pool optimized", LogLevel::Info);
                else if (r < 80) currentEngine->addLogLine("NET_MANAGER: Package loss 0.2%", LogLevel::Warning);
                else if (r < 95) currentEngine->addLogLine("API_CORE: Error 500 on /v1/auth", LogLevel::Error);
                else currentEngine->addLogLine("FATAL: CPU STACK OVERFLOW", LogLevel::Critical);
            }
        }

        // --- ОКНО УПРАВЛЕНИЯ ---
        ImGui::SetNextWindowPos(ImVec2(10, 10), ImGuiCond_FirstUseEver);
        ImGui::SetNextWindowSize(ImVec2(350, 400), ImGuiCond_FirstUseEver);
        ImGui::Begin("Control Panel");
            
            ImGui::Text("Engine Mode:");
            if (ImGui::RadioButton("Naive (Slow)", currentEngine == &naive)) currentEngine = &naive;
            ImGui::SameLine();
            if (ImGui::RadioButton("Flyweight (Fast)", currentEngine == &flyweight)) currentEngine = &flyweight;

            ImGui::Separator();
            if (ImGui::Button(systemRunning ? "PAUSE SYSTEM" : "START SYSTEM", ImVec2(-1, 40))) systemRunning = !systemRunning;
            
            if (ImGui::Button("INSTANT FLOOD (10k logs)", ImVec2(-1, 40))) {
                for(int i=0; i<10000; i++) currentEngine->addLogLine("Flood message " + std::to_string(i), LogLevel::Info);
            }

            ImGui::Separator();
            float curMem = currentEngine->getMemoryUsage() / 1024.0f / 1024.0f;
            if (curMem > maxMem) maxMem = curMem;
            ImGui::Text("Logs: %zu", currentEngine->getLineCount());
            ImGui::Text("RAM: %.2f MB", curMem);

            if (memHistory.size() > 100) memHistory.erase(memHistory.begin());
            memHistory.push_back(curMem);
            ImGui::PlotLines("Memory", memHistory.data(), (int)memHistory.size(), 0, NULL, 0.0f, maxMem * 1.5f, ImVec2(-1, 80));

        ImGui::End();

        // --- ОКНО ЛОГОВ (ТО, ЧЕГО НЕ ХВАТАЛО) ---
        ImGui::SetNextWindowPos(ImVec2(370, 10), ImGuiCond_FirstUseEver);
        ImGui::SetNextWindowSize(ImVec2(880, 680), ImGuiCond_FirstUseEver);
        ImGui::Begin("Log Explorer");
            
            if (ImGui::BeginChild("ScrollRegion")) {
                // Используем Clipper, чтобы не лагало
                ImGuiListClipper clipper;
                clipper.Begin((int)currentEngine->getLineCount());
                while (clipper.Step()) {
                    for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++) {
                        LogLevel lvl = currentEngine->getLineLevel(i);
                        std::string msg = currentEngine->getLineMessage(i);

                        ImVec4 col;
                        if (lvl == LogLevel::Info) col = ImVec4(1, 1, 1, 1);
                        else if (lvl == LogLevel::Warning) col = ImVec4(1, 1, 0, 1);
                        else if (lvl == LogLevel::Error) col = ImVec4(1, 0.4f, 0.4f, 1);
                        else col = ImVec4(1, 0, 0, 1);

                        ImGui::TextColored(ImVec4(0.5f, 0.5f, 0.5f, 1), "[%05d]", i);
                        ImGui::SameLine();
                        ImGui::TextColored(col, "%s", msg.c_str());
                    }
                }
                if (ImGui::GetScrollY() >= ImGui::GetScrollMaxY()) ImGui::SetScrollHereY(1.0f);
            }
            ImGui::EndChild();
        ImGui::End();

        // Рендеринг
        ImGui::Render();
        int display_w, display_h;
        glfwGetFramebufferSize(window, &display_w, &display_h);
        glViewport(0, 0, display_w, display_h);
        glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT);
        ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
        glfwSwapBuffers(window);
    }

    // Очистка
    ImGui_ImplOpenGL3_Shutdown();
    ImGui_ImplGlfw_Shutdown();
    ImGui::DestroyContext();
    glfwDestroyWindow(window);
    glfwTerminate();

    return 0;
}