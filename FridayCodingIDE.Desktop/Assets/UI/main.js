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

    // Fix Alignment/Typing Line issues by forcing resize
    const resizeObserver = new ResizeObserver(() => {
        editor.resize();
    });
    resizeObserver.observe(document.getElementById('lua-editor'));

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
    
    sendToCSharp({ action: "run_mod", code: code });
});

// Install Psych Button
const installBtn = document.getElementById('installBtn');
installBtn.addEventListener('click', () => {
    log("Starting Psych Engine Installation...", "success");
    log("Downloading from GameBanana...", "progress", "install-step");
    
    sendToCSharp({ action: "install_psych", url: "https://gamebanana.com/mods/download/309789#FileInfo_1406924" });
});

function sendToCSharp(payload) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage(payload);
    } else if (typeof invokeCSharpAction === 'function') {
        invokeCSharpAction(JSON.stringify(payload));
    }
}

function log(message, type = "", id = "") {
    const consoleElem = document.getElementById('console');
    
    // If id is provided and element exists, update it instead of creating new
    if (id && document.getElementById(id)) {
        const existing = document.getElementById(id);
        existing.textContent = `[${new Date().toLocaleTimeString()}] ${message}`;
        existing.className = `log ${type ? 'log-' + type : ''}`;
        return;
    }

    const logElem = document.createElement('div');
    if (id) logElem.id = id;
    logElem.className = `log ${type ? 'log-' + type : ''}`;
    
    if (type === "progress") {
        const spinner = document.createElement('div');
        spinner.className = 'log-spinner';
        logElem.appendChild(spinner);
    }

    const text = document.createElement('span');
    text.textContent = `[${new Date().toLocaleTimeString()}] ${message}`;
    logElem.appendChild(text);

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
