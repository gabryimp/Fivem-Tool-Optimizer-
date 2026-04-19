#include "imgui.h"

// Function prototypes for the UI callbacks
void RenderWindow1();
void RenderWindow2();

// Main UI function
void RenderUI() {
    // Create main window
    ImGui::Begin("FiveM Optimizer");

    // Callback functions for UI elements
    RenderWindow1();
    RenderWindow2();

    ImGui::End();
}

// Example window rendering function
void RenderWindow1() {
    if (ImGui::Begin("Window 1")) {
        ImGui::Text("This is the first control window.");
        static int option = 0;
        ImGui::RadioButton("Option 1", &option, 0);
        ImGui::RadioButton("Option 2", &option, 1);
        ImGui::End();
    }
}

// Another example window rendering function
void RenderWindow2() {
    if (ImGui::Begin("Window 2")) {
        ImGui::Text("This is the second control window.");
        if (ImGui::Button("Execute Action")) {
            // Callback for the button
        }
        ImGui::End();
    }
}