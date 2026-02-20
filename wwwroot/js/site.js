(() => {
    const button = document.getElementById('menu-toggle');

    if (!button) {
        return;
    }

    const collapseClass = 'sidebar-collapsed';
    const storageKey = 'painel-menu-collapsed';

    const applyState = (collapsed) => {
        document.body.classList.toggle(collapseClass, collapsed);
        button.setAttribute('aria-expanded', (!collapsed).toString());
    };

    const saved = localStorage.getItem(storageKey) === 'true';
    applyState(saved);

    button.addEventListener('click', () => {
        const collapsed = !document.body.classList.contains(collapseClass);
        applyState(collapsed);
        localStorage.setItem(storageKey, collapsed.toString());
    });
})();
