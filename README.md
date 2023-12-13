# FoundationModelMapGenerator

Based on a user text prompt, generates a map of tiles for use in a Unity application.

## Details
* This is basic prototyping to show proof of concept. This approach works but needs significant improvement to be preferable over manually creating maps.
* See Roadmap.txt for possible future steps
* Connects to foundation model (ChatGPT or Bard) and returns JSON that is converted into a map of tiles
* Based on old code for a Tactics-genre game

## Examples
* Prompt: "A 3x3 square"
* ![Alt text](/examples/foundation_model_3x3.png)
* Prompt: "A 8x8 checkerboard with the black tiles 5 times the height of the white tiles"
* ![Alt text](/examples/foundation_model_checkerboard.png)
* Prompt: "A large square with the middle missing"
* ![Alt text](/examples/large_square_middle_missing.png)

## Failures / Improvement
* Follow up conversations lead to the LLM responding in non-JSON. Model keeps apologizing in non-JSON format rather than generating maps.
* Does not handle abstract conversations well and attempt to build a map. Instead sends a non-JSON explanation. Example "Generate a map like Florida."
* Many of the simple outputs are not that great (generate a circle, a triangle, the letter J etc).
