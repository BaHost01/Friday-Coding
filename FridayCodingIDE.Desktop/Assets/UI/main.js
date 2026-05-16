let editor;

window.addEventListener('DOMContentLoaded', () => {
    // Initialize Ace Editor
    editor = ace.edit("lua-editor");
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/lua");
    editor.setShowPrintMargin(false);
    editor.setOptions({
        enableBasicAutocompletion: true,
        enableLiveAutocompletion: true,
        fontSize: "14px"
    });

    // Loading Sequence
    setTimeout(() => {
        const overlay = document.getElementById('loading-overlay');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                overlay.style.display = 'none';
            }, 500);
        }
    }, 1500);
});

// Run Mod Button
const runBtn = document.getElementById('runBtn');
runBtn.addEventListener('click', () => {
    const code = editor.getValue();
    log("Executing Lua script...", "success");
    
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({ action: "run_mod", code: code });
    } else if (typeof invokeCSharpAction === 'function') {
        invokeCSharpAction(JSON.stringify({ action: "run_mod", code: code }));
    }
});

function log(message, type = "") {
    const consoleElem = document.getElementById('console');
    const logElem = document.createElement('div');
    logElem.className = `log ${type ? 'log-' + type : ''}`;
    logElem.textContent = `[${new Date().toLocaleTimeString()}] ${message}`;
    consoleElem.appendChild(logElem);
    consoleElem.scrollTop = consoleElem.scrollHeight;
}

// Interface for C# to call
window.ide = {
    setProjectFiles: (files) => {
        const list = document.getElementById('file-list');
        list.innerHTML = '';
        files.forEach(file => {
            const li = document.createElement('li');
            li.textContent = file;
            li.className = 'file-item';
            li.onclick = () => log(`Opened ${file}`, "success");
            list.appendChild(li);
        });
    },
    appendLog: (message, type) => log(message, type),
    setCode: (code) => {
        editor.setValue(code, -1);
    }
};
