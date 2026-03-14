cmake_minimum_required(VERSION 3.14)
project(LogAnalyzer)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# --- БЛОК АВТОМАТИЧЕСКОЙ ЗАГРУЗКИ БИБЛИОТЕК ---
include(FetchContent)

FetchContent_Declare(
  glfw
  GIT_REPOSITORY https://github.com/glfw/glfw.git
  GIT_TAG 3.3.8
)
set(GLFW_BUILD_DOCS OFF CACHE BOOL "" FORCE)
set(GLFW_INSTALL OFF CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(glfw)

# 2. Загружаем Dear ImGui (Интерфейс)
FetchContent_Declare(
  imgui
  GIT_REPOSITORY https://github.com/ocornut/imgui.git
  GIT_TAG v1.90
)
FetchContent_GetProperties(imgui)
if(NOT imgui_POPULATED)
    FetchContent_Populate(imgui)
    set(IMGUI_DIR ${imgui_SOURCE_DIR})
endif()

# --- СПИСОК ФАЙЛОВ ПРОЕКТА ---
set(SOURCE_FILES 
    src/main.cpp 
    src/NaiveEngine/NaiveEngine.cpp 
    src/FlyweightEngine/FlyweightEngine.cpp 
    src/UI/LogWindow.cpp
    ${IMGUI_DIR}/imgui.cpp
    ${IMGUI_DIR}/imgui_draw.cpp
    ${IMGUI_DIR}/imgui_widgets.cpp
    ${IMGUI_DIR}/imgui_tables.cpp
    ${IMGUI_DIR}/imgui_demo.cpp
    ${IMGUI_DIR}/backends/imgui_impl_glfw.cpp
    ${IMGUI_DIR}/backends/imgui_impl_opengl3.cpp
)

add_executable(${PROJECT_NAME} ${SOURCE_FILES})

# --- НАСТРОЙКА ПУТЕЙ И ЛИНКОВКА ---

target_include_directories(${PROJECT_NAME} PUBLIC 
    include 
    ${IMGUI_DIR}
    ${IMGUI_DIR}/backends
)

target_link_libraries(${PROJECT_NAME} PUBLIC glfw opengl32)