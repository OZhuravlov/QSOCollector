// QSOCollector User Manual - Navigation & Interactivity

/**
 * Toggle submenu visibility
 * @param {HTMLElement} element - The menu-toggle element that was clicked
 */
function toggleMenu(element, e) {
    // Prevent default behavior if this is called from an event
    if (e) {
        e.preventDefault();
        e.stopPropagation();
    }

    const submenu = element.nextElementSibling;
    if (submenu && submenu.classList.contains('submenu')) {
        const isVisible = submenu.style.display !== 'none';
        submenu.style.display = isVisible ? 'none' : 'block';

        // Update toggle state class
        if (isVisible) {
            element.classList.remove('expanded');
        } else {
            element.classList.add('expanded');
        }

        // Update arrow icon if present
        const arrow = element.querySelector('.menu-arrow');
        if (arrow) {
            arrow.textContent = isVisible ? '▶' : '▼';
        }
    }
}

/**
 * Load and display specific section
 */
function loadSection(sectionId) {
    const sections = document.querySelectorAll('.section');

    sections.forEach(section => {
        section.classList.remove('active');
        section.style.display = 'none';
    });

    const targetSection = document.getElementById(sectionId);
    if (targetSection) {
        targetSection.classList.add('active');
        targetSection.style.display = 'block';
        window.scrollTo(0, 0);
        updateActiveMenu(sectionId);
    }
}

/**
 * Update active menu item
 */
function updateActiveMenu(sectionId) {
    const menuItems = document.querySelectorAll('.toc a');
    menuItems.forEach(item => {
        item.style.fontWeight = 'normal';
        item.style.borderLeftColor = 'transparent';
    });

    const activeItem = document.querySelector(`.toc a[href="#${sectionId}"]`);
    if (activeItem) {
        activeItem.style.fontWeight = '600';
        activeItem.style.borderLeftColor = 'var(--primary-color)';
    }
}

/**
 * Scroll to top
 */
function goToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

/**
 * Initialize event listeners
 */
document.addEventListener('DOMContentLoaded', function() {
    // Setup menu toggle handlers (remove duplicate event listeners)
    // The toggleMenu is already called via inline onclick in HTML
    // Here we just ensure proper behavior
    const menuToggles = document.querySelectorAll('.menu-toggle');
    menuToggles.forEach(toggle => {
        // Set cursor to pointer
        toggle.style.cursor = 'pointer';
        // Add accessible role
        toggle.setAttribute('role', 'button');
        toggle.setAttribute('tabindex', '0');

        // Handle Enter/Space key for keyboard accessibility
        toggle.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggleMenu(this, e);
            }
        });
    });

    // Setup keyboard shortcuts
    setupKeyboardShortcuts();

    // Setup search functionality if available
    setupSearch();

    // Print support
    setupPrintSupport();

    console.log('QSOCollector User Manual loaded successfully');
});

/**
 * Setup keyboard shortcuts
 */
function setupKeyboardShortcuts() {
    document.addEventListener('keydown', function(e) {
        // Ctrl+Home or Cmd+Home = Go to top
        if ((e.ctrlKey || e.metaKey) && e.key === 'Home') {
            goToTop();
        }

        // Ctrl+F or Cmd+F = Focus search (if search implemented)
        if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
            const searchBox = document.getElementById('search-box');
            if (searchBox) {
                e.preventDefault();
                searchBox.focus();
            }
        }
    });
}

/**
 * Setup search functionality (basic text search)
 */
function setupSearch() {
    const searchBox = document.getElementById('search-box');
    if (!searchBox) return;

    searchBox.addEventListener('input', function(e) {
        const query = e.target.value.toLowerCase();
        const sections = document.querySelectorAll('.section');

        if (query.length === 0) {
            // Show first section if search cleared
            sections.forEach((section, index) => {
                section.style.display = index === 0 ? 'block' : 'none';
            });
            return;
        }

        // Search in all sections
        let foundAny = false;
        sections.forEach(section => {
            const text = section.textContent.toLowerCase();
            if (text.includes(query)) {
                section.style.display = 'block';
                section.classList.add('active');
                foundAny = true;
            } else {
                section.style.display = 'none';
                section.classList.remove('active');
            }
        });
    });
}

/**
 * Setup print support
 */
function setupPrintSupport() {
    // Add print button if available
    const printButton = document.getElementById('print-button');
    if (printButton) {
        printButton.addEventListener('click', function() {
            window.print();
        });
    }
}

/**
 * Expand all menus
 */
function expandAllMenus() {
    const submenus = document.querySelectorAll('.submenu');
    const toggles = document.querySelectorAll('.menu-toggle');

    submenus.forEach(submenu => {
        submenu.style.display = 'block';
    });

    toggles.forEach(toggle => {
        toggle.classList.add('expanded');
    });
}

/**
 * Collapse all menus
 */
function collapseAllMenus() {
    const submenus = document.querySelectorAll('.submenu');
    const toggles = document.querySelectorAll('.menu-toggle');

    submenus.forEach(submenu => {
        submenu.style.display = 'none';
    });

    toggles.forEach(toggle => {
        toggle.classList.remove('expanded');
    });
}

/**
 * Get section reading time estimate
 */
function getReadingTimeEstimate() {
    const section = document.querySelector('.section.active');
    if (!section) return 0;

    const text = section.textContent;
    const wordCount = text.split(/\s+/).length;
    const wordsPerMinute = 200;
    return Math.ceil(wordCount / wordsPerMinute);
}

/**
 * Highlight search terms in text
 */
function highlightText(text, searchTerm) {
    if (!searchTerm) return text;

    const regex = new RegExp(`(${searchTerm})`, 'gi');
    return text.replace(regex, '<mark>$1</mark>');
}

/**
 * Export current section as text
 */
function exportSection() {
    const section = document.querySelector('.section.active');
    if (!section) return;

    const h2 = section.querySelector('h2');
    const title = h2 ? h2.textContent : 'QSOCollector Manual';
    const content = section.textContent;

    const element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(content));
    element.setAttribute('download', `qsocollector-${title.toLowerCase().replace(/\s+/g, '-')}.txt`);
    element.style.display = 'none';

    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
}

/**
 * Table of Contents Generator (if needed)
 */
function generateTableOfContents() {
    const headings = document.querySelectorAll('.section.active h2, .section.active h3');
    const toc = [];

    headings.forEach(heading => {
        const level = heading.tagName === 'H2' ? 1 : 2;
        toc.push({
            text: heading.textContent,
            level: level,
            id: heading.id || ''
        });
    });

    return toc;
}

/**
 * Breadcrumb navigation
 */
function getCurrentBreadcrumb() {
    const activeSection = document.querySelector('.section.active');
    if (!activeSection) return [];

    const h2 = activeSection.querySelector('h2');
    if (!h2) return ['Home'];

    return ['Home', h2.textContent];
}

// Make functions globally available
window.toggleMenu = toggleMenu;
window.loadSection = loadSection;
window.goToTop = goToTop;
window.expandAllMenus = expandAllMenus;
window.collapseAllMenus = collapseAllMenus;
window.exportSection = exportSection;
window.getReadingTimeEstimate = getReadingTimeEstimate;
