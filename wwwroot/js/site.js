(() => {
    const menuButton = document.getElementById('menu-toggle');
    const themeSelect = document.getElementById('theme-select');

    const collapseClass = 'sidebar-collapsed';
    const themeDarkClass = 'theme-dark';
    const menuStorageKey = 'painel-menu-collapsed';
    const themeStorageKey = 'painel-theme';

    const applyMenuState = (collapsed) => {
        document.body.classList.toggle(collapseClass, collapsed);
        if (menuButton) {
            menuButton.setAttribute('aria-expanded', (!collapsed).toString());
        }
    };

    const applyTheme = (theme) => {
        const isDark = theme === 'dark';
        document.body.classList.toggle(themeDarkClass, isDark);
        if (themeSelect) {
            themeSelect.value = isDark ? 'dark' : 'light';
        }
    };

    const savedMenuCollapsed = localStorage.getItem(menuStorageKey) === 'true';
    applyMenuState(savedMenuCollapsed);

    const savedTheme = localStorage.getItem(themeStorageKey) || 'light';
    applyTheme(savedTheme);

    if (menuButton) {
        menuButton.addEventListener('click', () => {
            const collapsed = !document.body.classList.contains(collapseClass);
            applyMenuState(collapsed);
            localStorage.setItem(menuStorageKey, collapsed.toString());
        });
    }

    if (themeSelect) {
        themeSelect.addEventListener('change', () => {
            const theme = themeSelect.value === 'dark' ? 'dark' : 'light';
            applyTheme(theme);
            localStorage.setItem(themeStorageKey, theme);
        });
    }
})();
