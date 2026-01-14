// Dropdown functionality to replace Bootstrap dropdowns
document.addEventListener('DOMContentLoaded', function () {
    // Get all dropdown toggles
    const dropdownToggles = document.querySelectorAll('[data-dropdown-toggle]');

    dropdownToggles.forEach(function (toggle) {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const dropdown = this.closest('.dropdown');
            const menu = dropdown.querySelector('.dropdown-menu');

            // Close other open dropdowns
            document.querySelectorAll('.dropdown-menu.show').forEach(function (otherMenu) {
                if (otherMenu !== menu) {
                    otherMenu.classList.remove('show');
                    otherMenu.previousElementSibling.setAttribute('aria-expanded', 'false');
                }
            });

            // Toggle current dropdown
            const isOpen = menu.classList.contains('show');
            if (isOpen) {
                menu.classList.remove('show');
                this.setAttribute('aria-expanded', 'false');
            } else {
                menu.classList.add('show');
                this.setAttribute('aria-expanded', 'true');
            }
        });
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.dropdown')) {
            document.querySelectorAll('.dropdown-menu.show').forEach(function (menu) {
                menu.classList.remove('show');
                const toggle = menu.previousElementSibling;
                if (toggle) {
                    toggle.setAttribute('aria-expanded', 'false');
                }
            });
        }
    });

    // Prevent dropdown from closing when clicking inside menu
    document.querySelectorAll('.dropdown-menu').forEach(function (menu) {
        menu.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    });
});
