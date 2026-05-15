window.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        const overlay = document.getElementById('loading-overlay');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                overlay.style.display = 'none';
            }, 500);
        }
    }, 2000);
});

document.querySelectorAll('.tab').forEach(button => {
    button.addEventListener('click', () => {
        const panel = button.closest('.panel');
        const tabName = button.getAttribute('data-tab');

        // Update buttons
        panel.querySelectorAll('.tab').forEach(b => b.classList.remove('active'));
        button.classList.add('active');

        // Update content
        panel.querySelectorAll('.tab-content').forEach(content => {
            content.classList.remove('active');
            if (content.id === tabName || content.id === tabName + '-explorer' || (tabName === 'explorer' && content.id === 'project-explorer')) {
                content.classList.add('active');
            }
        });
    });
});

const runBtn = document.getElementById('runBtn');
runBtn.addEventListener('click', () => {
    log("Running Mod...", "success");
    if (typeof invokeCSharpAction === 'function') {
        invokeCSharpAction(JSON.stringify({ action: "run_mod" }));
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
            list.appendChild(li);
        });
    },
    appendLog: (message, type) => log(message, type)
};
