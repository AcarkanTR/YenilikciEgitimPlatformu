/*
 * ════════════════════════════════════════════════════════════════════════════
 * Theme Toggle - Dark/Light Mode Yönetimi
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * LocalStorage'da tema tercihi saklanır
 * Sayfa yüklendiğinde otomatik uygulanır (flash önleme)
 */

export const ThemeManager = {

    /**
     * Başlangıç - Sayfa yüklenirken çağrılır
     */
    init() {
        // LocalStorage'dan tema oku, yoksa sistem tercihini al
        const savedTheme = localStorage.getItem('theme');
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

        const theme = savedTheme || (systemPrefersDark ? 'dark' : 'light');
        this.setTheme(theme, false); // Animasyon yok (flash önleme)

        // Sistem teması değişirse dinle
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem('theme')) {
                this.setTheme(e.matches ? 'dark' : 'light');
            }
        });
    },

    /**
     * Temayı değiştir
     * @param {string} theme - 'light' veya 'dark'
     * @param {boolean} animate - Geçiş animasyonu
     */
    setTheme(theme, animate = true) {
        const html = document.documentElement;

        if (theme === 'dark') {
            html.classList.add('dark');
        } else {
            html.classList.remove('dark');
        }

        localStorage.setItem('theme', theme);
        this.updateToggleButton(theme);

        // Animate
        if (animate) {
            html.style.transition = 'background-color 0.3s ease, color 0.3s ease';
            setTimeout(() => {
                html.style.transition = '';
            }, 300);
        }
    },

    /**
     * Toggle - Light ↔ Dark
     */
    toggle() {
        const currentTheme = document.documentElement.classList.contains('dark') ? 'dark' : 'light';
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        this.setTheme(newTheme, true);
    },

    /**
     * Mevcut temayı al
     * @returns {string} 'light' veya 'dark'
     */
    getCurrentTheme() {
        return document.documentElement.classList.contains('dark') ? 'dark' : 'light';
    },

    /**
     * Toggle butonunu güncelle
     * @param {string} theme - Mevcut tema
     */
    updateToggleButton(theme) {
        const toggleBtn = document.getElementById('theme-toggle');
        const iconLight = document.getElementById('theme-toggle-light-icon');
        const iconDark = document.getElementById('theme-toggle-dark-icon');

        if (!toggleBtn) return;

        if (theme === 'dark') {
            iconLight?.classList.remove('hidden');
            iconDark?.classList.add('hidden');
        } else {
            iconLight?.classList.add('hidden');
            iconDark?.classList.remove('hidden');
        }
    }
};

// Auto-init (sayfa yüklenirken flash önlemek için)
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => ThemeManager.init());
} else {
    ThemeManager.init();
}

export default ThemeManager;