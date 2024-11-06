cmake_minimum_required(VERSION 3.10)

# Set the project name and output type
project(MonoPatcher LANGUAGES CXX)

# Specify the version of C++ to use
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# Use FetchContent to download and include MinHook
include(FetchContent)

FetchContent_Declare(
  MinHook
  GIT_REPOSITORY https://github.com/TsudaKageyu/minhook.git
  GIT_TAG        master  # You can change this to a specific tag or commit
)

FetchContent_MakeAvailable(MinHook)

# Add all source and header files from the src and include directories
file(GLOB_RECURSE SOURCES "src/*.cpp" "src/*/*.cpp")
file(GLOB_RECURSE HEADERS "include/*.h" "include/*/*.h")

# Define the output as a DLL
add_library(MonoPatcher SHARED ${SOURCES} ${HEADERS})

# Include directories for headers
target_include_directories(MonoPatcher PRIVATE include minhook/include)

# Link the MinHook static library
target_link_libraries(MonoPatcher PRIVATE minhook)

# Set the output directory for the DLL and the import library
set_target_properties(MonoPatcher PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib"
    SUFFIX ".asi"  # Change the extension from .dll to .asi
)

# Optional: define additional preprocessor macros if needed
target_compile_definitions(MonoPatcher PRIVATE MONO_PATCHER_EXPORTS)
