/**
 * Loads a code snippet from a file and injects it into a code block by id.
 * @param {string} filePath - Path to the code file (relative to HTML page).
 * @param {string} codeBlockId - The id of the <code> element to inject into.
 */
function loadCodeSnippet(filePath, codeBlockId) {
  fetch(filePath)
    .then(response => response.text())
    .then(code => {
      const codeBlock = document.getElementById(codeBlockId);
      if (codeBlock) {
        codeBlock.textContent = code;
        if (window.Prism) Prism.highlightElement(codeBlock);
      }
    })
    .catch(() => {
      const codeBlock = document.getElementById(codeBlockId);
      if (codeBlock) codeBlock.textContent = '// Code block unavailable.';
    });
}
window.loadCodeSnippet = loadCodeSnippet;
    