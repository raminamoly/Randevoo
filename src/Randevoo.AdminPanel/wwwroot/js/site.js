$(function () {
    document.querySelectorAll("i.bi:not([aria-label])").forEach(function (icon) {
        icon.setAttribute("aria-hidden", "true");
    });

    const sidebar = document.getElementById("sidebarCollapse");
    const adminShell = document.querySelector(".admin-shell");
    const desktopSidebarQuery = window.matchMedia("(min-width: 992px)");
    const isDesktopSidebarCollapsed = function () {
        return Boolean(adminShell && adminShell.classList.contains("sidebar-collapsed") && desktopSidebarQuery.matches);
    };

    const getNavLabel = function (element) {
        return (element.innerText || element.getAttribute("aria-label") || element.getAttribute("title") || "")
            .replace(/\s+/g, " ")
            .trim();
    };

    const setSidebarCollapsed = function (collapsed) {
        if (!adminShell || !desktopSidebarQuery.matches) {
            return;
        }

        adminShell.classList.toggle("sidebar-collapsed", collapsed);
        localStorage.setItem("randevoo.sidebarCollapsed", collapsed ? "true" : "false");
        syncSidebarRail();
    };

    const syncSidebarRail = function () {
        if (!adminShell) {
            return;
        }

        const collapsed = isDesktopSidebarCollapsed();
        document.querySelectorAll("[data-sidebar-rail-toggle='true']").forEach(function (button) {
            button.setAttribute("aria-expanded", collapsed ? "false" : "true");
            button.setAttribute("aria-label", collapsed ? "باز کردن منو" : "کوچک کردن منو");
            button.setAttribute("title", collapsed ? "باز کردن منو" : "کوچک کردن منو");
            const icon = button.querySelector("i.bi");
            if (icon) {
                icon.className = "bi bi-list";
            }
        });

        if (!sidebar) {
            return;
        }

        sidebar.querySelectorAll(".sidebar-nav a.nav-link, .sidebar-nav .nav-tree-toggle").forEach(function (item) {
            const label = getNavLabel(item);
            if (label) {
                item.setAttribute("title", label);
                item.setAttribute("aria-label", label);
            }

            if (!item.matches("a.nav-link") || !window.bootstrap || !window.bootstrap.Tooltip) {
                return;
            }

            const existing = window.bootstrap.Tooltip.getInstance(item);
            if (existing) {
                existing.dispose();
            }

            if (collapsed && label) {
                item.setAttribute("data-bs-toggle", "tooltip");
                item.setAttribute("data-bs-placement", "left");
                new window.bootstrap.Tooltip(item, { trigger: "hover focus" });
            } else {
                item.removeAttribute("data-bs-toggle");
                item.removeAttribute("data-bs-placement");
            }
        });
    };

    const closeOtherSidebarSections = function (currentPanel) {
        if (!sidebar || !window.bootstrap || !window.bootstrap.Collapse) {
            return;
        }

        sidebar.querySelectorAll(".nav-subnav.show").forEach(function (panel) {
            if (panel === currentPanel) {
                return;
            }

            window.bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).hide();
        });
    };

    if (adminShell) {
        const storedRailState = localStorage.getItem("randevoo.sidebarCollapsed");
        if (storedRailState === "true" && desktopSidebarQuery.matches) {
            adminShell.classList.add("sidebar-collapsed");
        }

        document.querySelectorAll("[data-sidebar-rail-toggle='true']").forEach(function (button) {
            button.addEventListener("click", function () {
                if (!desktopSidebarQuery.matches) {
                    return;
                }

                setSidebarCollapsed(!adminShell.classList.contains("sidebar-collapsed"));
            });
        });

        desktopSidebarQuery.addEventListener("change", function (event) {
            if (!event.matches) {
                adminShell.classList.remove("sidebar-collapsed");
            } else if (localStorage.getItem("randevoo.sidebarCollapsed") === "true") {
                adminShell.classList.add("sidebar-collapsed");
            }

            syncSidebarRail();
        });

        syncSidebarRail();
    }

    if (sidebar && window.bootstrap && window.bootstrap.Collapse) {
        const sidebarController = window.bootstrap.Collapse.getOrCreateInstance(sidebar, {
            toggle: false
        });

        sidebar.querySelectorAll(".nav-subnav").forEach(function (panel) {
            panel.addEventListener("show.bs.collapse", function () {
                closeOtherSidebarSections(panel);
            });
        });

        const activeOpenPanels = Array.prototype.slice.call(sidebar.querySelectorAll(".nav-subnav.show"));
        activeOpenPanels.slice(1).forEach(function (panel) {
            window.bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).hide();
        });

        sidebar.querySelectorAll(".sidebar-nav a.nav-link, .sidebar-nav .nav-tree-toggle").forEach(function (item) {
            item.addEventListener("click", function (event) {
                if (!isDesktopSidebarCollapsed()) {
                    return;
                }

                event.preventDefault();
                event.stopPropagation();
                setSidebarCollapsed(false);
            });
        });

        sidebar.querySelectorAll(".sidebar-nav a.nav-link").forEach(function (link) {
            link.addEventListener("click", function () {
                if (window.matchMedia("(max-width: 991.98px)").matches && sidebar.classList.contains("show")) {
                    sidebarController.hide();
                }
            });
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && window.matchMedia("(max-width: 991.98px)").matches && sidebar.classList.contains("show")) {
                sidebarController.hide();
            }
        });

        const syncMobileSidebarState = function () {
            const isMobileOpen = window.matchMedia("(max-width: 991.98px)").matches && sidebar.classList.contains("show");
            document.body.classList.toggle("sidebar-mobile-open", isMobileOpen);
            if (adminShell) {
                adminShell.classList.toggle("sidebar-mobile-open", isMobileOpen);
            }
        };

        sidebar.addEventListener("shown.bs.collapse", syncMobileSidebarState);
        sidebar.addEventListener("hidden.bs.collapse", syncMobileSidebarState);

        document.addEventListener("click", function (event) {
            if (!window.matchMedia("(max-width: 991.98px)").matches || !sidebar.classList.contains("show")) {
                return;
            }

            const target = event.target;
            if (sidebar.contains(target) || target.closest("[data-bs-target='#sidebarCollapse']")) {
                return;
            }

            sidebarController.hide();
        });

        window.addEventListener("resize", syncMobileSidebarState);
        syncMobileSidebarState();
    }

    if (window.bootstrap && window.bootstrap.Tooltip) {
        document.querySelectorAll("[data-bs-toggle='tooltip']").forEach(function (element) {
            const instance = window.bootstrap.Tooltip.getInstance(element);
            if (instance) {
                instance.dispose();
            }

            new window.bootstrap.Tooltip(element);
        });
    }

    if (window.bootstrap && window.bootstrap.Dropdown) {
        const positionFloatingActionMenu = function (toggle, menu) {
            if (!toggle || !menu) {
                return;
            }

            const gap = 8;
            const padding = 12;
            const rect = toggle.getBoundingClientRect();
            const menuWidth = menu.offsetWidth || 220;
            const menuHeight = menu.offsetHeight || 0;
            const alignEnd = menu.classList.contains("dropdown-menu-end");
            const viewportWidth = window.innerWidth || document.documentElement.clientWidth;
            const viewportHeight = window.innerHeight || document.documentElement.clientHeight;

            let left = alignEnd ? rect.right - menuWidth : rect.left;
            left = Math.max(padding, Math.min(left, viewportWidth - menuWidth - padding));

            let top = rect.bottom + gap;
            if (menuHeight > 0 && top + menuHeight > viewportHeight - padding) {
                top = Math.max(padding, rect.top - menuHeight - gap);
            }

            menu.style.position = "fixed";
            menu.style.inset = "auto auto auto auto";
            menu.style.left = `${left}px`;
            menu.style.top = `${top}px`;
            menu.style.right = "auto";
            menu.style.bottom = "auto";
            menu.style.transform = "none";
            menu.style.zIndex = "3200";
        };

        const restoreFloatingActionMenu = function (toggle, menu) {
            const ownerId = toggle && toggle.dataset.actionMenuOwner;
            const owner = ownerId ? document.getElementById(ownerId) : null;

            if (owner && menu && menu.parentElement !== owner) {
                owner.appendChild(menu);
            }

            if (menu) {
                menu.classList.remove("floating-action-menu");
                delete menu.dataset.floatingOwner;
                menu.style.position = "";
                menu.style.inset = "";
                menu.style.left = "";
                menu.style.top = "";
                menu.style.right = "";
                menu.style.bottom = "";
                menu.style.transform = "";
                menu.style.zIndex = "";
            }
        };

        document.querySelectorAll(".table-action-toggle[data-bs-toggle='dropdown']").forEach(function (element, index) {
            const instance = window.bootstrap.Dropdown.getInstance(element);
            if (instance) {
                instance.dispose();
            }

            const dropdown = element.closest(".dropdown");
            const menu = dropdown ? dropdown.querySelector(".dropdown-menu") : null;
            if (dropdown && menu && !dropdown.id) {
                dropdown.id = `table-action-dropdown-${Date.now()}-${index}`;
            }

            if (dropdown) {
                element.dataset.actionMenuOwner = dropdown.id;
            }

            new window.bootstrap.Dropdown(element, {
                display: "static",
                autoClose: true
            });

            element.addEventListener("show.bs.dropdown", function () {
                const currentMenu = dropdown ? dropdown.querySelector(".dropdown-menu") : null;
                if (!currentMenu) {
                    return;
                }

                currentMenu.classList.add("floating-action-menu");
                currentMenu.dataset.floatingOwner = dropdown.id;
                document.body.appendChild(currentMenu);
                requestAnimationFrame(function () {
                    positionFloatingActionMenu(element, currentMenu);
                });
            });

            element.addEventListener("shown.bs.dropdown", function () {
                const currentMenu = document.body.querySelector(`.floating-action-menu.show[data-floating-owner="${dropdown.id}"]`);
                positionFloatingActionMenu(element, currentMenu);
            });

            element.addEventListener("hidden.bs.dropdown", function () {
                const currentMenu = document.body.querySelector(`.floating-action-menu[data-floating-owner="${dropdown.id}"]`);
                restoreFloatingActionMenu(element, currentMenu);
            });
        });

        window.addEventListener("resize", function () {
            const openToggle = document.querySelector(".table-action-toggle.show");
            const openMenu = document.body.querySelector(".floating-action-menu.show");
            positionFloatingActionMenu(openToggle, openMenu);
        });

        window.addEventListener("scroll", function () {
            const openToggle = document.querySelector(".table-action-toggle.show");
            const openMenu = document.body.querySelector(".floating-action-menu.show");
            positionFloatingActionMenu(openToggle, openMenu);
        }, true);
    }

    $(".quill-editor").each(function () {
        const host = this;
        const inputSelector = host.dataset.input;
        const hiddenInput = inputSelector ? document.querySelector(inputSelector) : null;
        if (!host || !hiddenInput || !window.Quill) {
            return;
        }

        const quill = new Quill(host, {
            theme: "snow",
            placeholder: host.dataset.placeholder || "",
            modules: {
                toolbar: [
                    [{ header: [2, 3, false] }],
                    [{ align: [] }, { direction: "rtl" }],
                    ["bold", "italic", "underline", "strike"],
                    [{ list: "ordered" }, { list: "bullet" }],
                    ["blockquote", "link"],
                    ["clean"]
                ]
            }
        });

        quill.root.innerHTML = hiddenInput.value || "";
        quill.root.setAttribute("dir", "rtl");
        quill.root.setAttribute("lang", "fa");
        quill.format("align", "right");
        quill.format("direction", "rtl");

        const sync = function () {
            hiddenInput.value = quill.root.innerHTML === "<p><br></p>" ? "" : quill.root.innerHTML;
        };

        quill.on("text-change", sync);
        $(host).closest("form").on("submit", sync);
    });

    $(".image-input").on("change", function () {
        const input = this;
        const target = $(input).data("target");
        const file = input.files && input.files[0];

        if (!file || !target) {
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            const image = $(target);
            image.attr("src", e.target.result).removeClass("d-none");
            image.closest(".image-manager-preview-shell").removeClass("is-empty");
        };
        reader.readAsDataURL(file);
    });

    $(".image-clear-button").on("click", function () {
        const previewSelector = $(this).data("target");
        const hiddenSelector = $(this).data("hidden-target");
        const fileSelector = $(this).data("file-target");

        if (previewSelector) {
            const image = $(previewSelector);
            image.attr("src", "").addClass("d-none");
            image.closest(".image-manager-preview-shell").addClass("is-empty");
        }

        if (hiddenSelector) {
            $(hiddenSelector).val("");
        }

        if (fileSelector) {
            $(fileSelector).val("");
        }
    });

    $("[data-tag-input='true']").each(function () {
        const editor = this;
        const hiddenInput = editor.dataset.tagHiddenTarget ? document.querySelector(editor.dataset.tagHiddenTarget) : null;
        const previewTarget = editor.dataset.tagPreviewTarget ? document.querySelector(editor.dataset.tagPreviewTarget) : null;
        const entryInput = editor.dataset.tagEntryTarget ? document.querySelector(editor.dataset.tagEntryTarget) : null;
        const addButton = editor.querySelector("[data-tag-add-button='true']");
        if (!hiddenInput || !previewTarget || !entryInput) {
            return;
        }

        const normalizeTag = function (value) {
            return (value || "").trim();
        };

        const parseTags = function (value) {
            return (value || "")
                .split(/[,،\n\r]+/)
                .map(normalizeTag)
                .filter(function (item, index, list) {
                    return item.length > 0
                        && list.findIndex(function (candidate) { return candidate.toLowerCase() === item.toLowerCase(); }) === index;
                })
                .slice(0, 10);
        };

        let tags = parseTags(hiddenInput.value);

        const syncHiddenInput = function () {
            hiddenInput.value = tags.join("، ");
        };

        const addTag = function (rawValue) {
            const candidates = parseTags(rawValue);
            let hasChanges = false;

            candidates.forEach(function (candidate) {
                if (tags.length >= 10) {
                    return;
                }

                const exists = tags.some(function (item) {
                    return item.toLowerCase() === candidate.toLowerCase();
                });

                if (!exists) {
                    tags.push(candidate);
                    hasChanges = true;
                }
            });

            if (hasChanges) {
                syncHiddenInput();
                renderTags();
            }
        };

        const removeTag = function (tagToRemove) {
            tags = tags.filter(function (item) {
                return item.toLowerCase() !== tagToRemove.toLowerCase();
            });

            syncHiddenInput();
            renderTags();
        };

        const renderTags = function () {
            previewTarget.innerHTML = "";

            if (tags.length === 0) {
                previewTarget.innerHTML = '<span class="tag-preview-empty">هنوز تگی ثبت نشده است.</span>';
                return;
            }

            tags.forEach(function (tag) {
                const chip = document.createElement("span");
                chip.className = "event-chip event-chip-editable";

                const label = document.createElement("span");
                label.textContent = tag;
                chip.appendChild(label);

                const removeButton = document.createElement("button");
                removeButton.type = "button";
                removeButton.className = "event-chip-remove";
                removeButton.setAttribute("aria-label", "حذف تگ");
                removeButton.textContent = "×";
                removeButton.addEventListener("click", function () {
                    removeTag(tag);
                });
                chip.appendChild(removeButton);

                previewTarget.appendChild(chip);
            });
        };

        entryInput.addEventListener("keydown", function (event) {
            if (event.key === "Enter" || event.key === "," || event.key === "،") {
                event.preventDefault();
                addTag(entryInput.value);
                entryInput.value = "";
            }
        });

        entryInput.addEventListener("blur", function () {
            if (!entryInput.value.trim()) {
                return;
            }

            addTag(entryInput.value);
            entryInput.value = "";
        });

        if (addButton) {
            addButton.addEventListener("click", function () {
                addTag(entryInput.value);
                entryInput.value = "";
                entryInput.focus();
            });
        }

        syncHiddenInput();
        renderTags();
    });

    $("[data-tag-combo='true']").each(function () {
        const editor = this;
        const select = editor.dataset.tagSelectTarget ? document.querySelector(editor.dataset.tagSelectTarget) : null;
        const picker = editor.dataset.tagPickerTarget ? document.querySelector(editor.dataset.tagPickerTarget) : null;
        const previewTarget = editor.dataset.tagPreviewTarget ? document.querySelector(editor.dataset.tagPreviewTarget) : null;

        if (!select || !picker || !previewTarget) {
            return;
        }

        const getSelectedOptions = function () {
            return Array.from(select.options).filter(function (option) {
                return option.selected;
            });
        };

        const syncPickerOptions = function () {
            const selectedValues = new Set(getSelectedOptions().map(function (option) {
                return option.value;
            }));

            Array.from(picker.options).forEach(function (option) {
                if (!option.value) {
                    return;
                }

                option.hidden = selectedValues.has(option.value);
                option.disabled = selectedValues.has(option.value);
            });

            picker.value = "";
            picker.dispatchEvent(new Event("randevoo:refresh-searchable-select"));
        };

        const renderSelectedTags = function () {
            previewTarget.innerHTML = "";
            const selectedOptions = getSelectedOptions();

            if (selectedOptions.length === 0) {
                previewTarget.innerHTML = '<span class="tag-preview-empty">هنوز تگی انتخاب نشده است.</span>';
                syncPickerOptions();
                return;
            }

            selectedOptions.forEach(function (option) {
                const chip = document.createElement("span");
                chip.className = "event-chip event-chip-editable";

                const label = document.createElement("span");
                label.textContent = option.textContent;
                chip.appendChild(label);

                const removeButton = document.createElement("button");
                removeButton.type = "button";
                removeButton.className = "event-chip-remove";
                removeButton.setAttribute("aria-label", `حذف تگ ${option.textContent}`);
                removeButton.innerHTML = '<i class="bi bi-x" aria-hidden="true"></i>';
                removeButton.addEventListener("click", function () {
                    option.selected = false;
                    select.dispatchEvent(new Event("change", { bubbles: true }));
                });
                chip.appendChild(removeButton);

                previewTarget.appendChild(chip);
            });

            syncPickerOptions();
        };

        picker.addEventListener("change", function () {
            if (!picker.value) {
                return;
            }

            const option = Array.from(select.options).find(function (candidate) {
                return candidate.value === picker.value;
            });

            if (option) {
                option.selected = true;
            }

            select.dispatchEvent(new Event("change", { bubbles: true }));
        });

        select.addEventListener("change", renderSelectedTags);
        renderSelectedTags();
    });

    if ($.fn.persianDatepicker && $(".jalali-date-picker").length) {
        $(".jalali-date-picker").each(function () {
            this.setAttribute("inputmode", "numeric");
            this.setAttribute("dir", "ltr");
            this.setAttribute("title", this.getAttribute("title") || "قالب تاریخ شمسی: ۱۴۰۵/۰۳/۱۵");
        });

        $(".jalali-date-picker").persianDatepicker({
            autoClose: true,
            calendarType: "persian",
            format: "YYYY/MM/DD",
            initialValue: false,
            observer: true,
            persianDigit: true,
            responsive: true,
            navigator: {
                scroll: {
                    enabled: false
                }
            },
            toolbox: {
                calendarSwitch: {
                    enabled: false
                }
            }
        });
    }

    if ($.fn.persianDatepicker && $(".jalali-date-time-picker").length) {
        $(".jalali-date-time-picker").each(function () {
            this.setAttribute("inputmode", "numeric");
            this.setAttribute("dir", "ltr");
            this.setAttribute("title", this.getAttribute("title") || "قالب تاریخ و ساعت شمسی: ۱۴۰۵/۰۳/۱۵ ۱۸:۳۰");
        });

        $(".jalali-date-time-picker").persianDatepicker({
            autoClose: true,
            calendarType: "persian",
            format: "YYYY/MM/DD HH:mm",
            initialValue: false,
            observer: true,
            persianDigit: true,
            responsive: true,
            timePicker: {
                enabled: true,
                second: {
                    enabled: false
                },
                meridian: {
                    enabled: false
                }
            },
            navigator: {
                scroll: {
                    enabled: false
                }
            },
            toolbox: {
                calendarSwitch: {
                    enabled: false
                }
            }
        });
    }

    const normalizeNumericInput = function (value) {
        return String(value || "")
            .replace(/[۰-۹]/g, function (digit) {
                return "۰۱۲۳۴۵۶۷۸۹".indexOf(digit);
            })
            .replace(/[٠-٩]/g, function (digit) {
                return "٠١٢٣٤٥٦٧٨٩".indexOf(digit);
            })
            .replace(/,/g, "")
            .trim();
    };

    const formatMoneyInput = function (input) {
        const normalized = normalizeNumericInput(input.value).replace(/[^\d.]/g, "");
        if (!normalized) {
            input.value = "";
            return;
        }

        const parts = normalized.split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        input.value = parts.length > 1 ? `${parts[0]}.${parts.slice(1).join("")}` : parts[0];
    };

    document.querySelectorAll(".money-input").forEach(function (input) {
        formatMoneyInput(input);
        input.addEventListener("input", function () {
            formatMoneyInput(input);
        });

        const form = input.closest("form");
        if (form) {
            form.addEventListener("submit", function () {
                input.value = normalizeNumericInput(input.value);
            });
        }
    });

    const initSearchableSelect = function (select) {
        if (!select || select.dataset.searchableReady === "true") {
            return;
        }

        select.dataset.searchableReady = "true";
        const wrapper = document.createElement("div");
        wrapper.className = "searchable-select";
        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control searchable-select-input";
        input.autocomplete = "off";
        input.placeholder = select.dataset.searchPlaceholder || "جستجو";
        input.setAttribute("aria-label", input.placeholder);
        input.setAttribute("role", "combobox");
        input.setAttribute("aria-expanded", "false");
        const list = document.createElement("div");
        list.className = "searchable-select-list";
        list.setAttribute("role", "listbox");

        select.parentNode.insertBefore(wrapper, select);
        wrapper.appendChild(input);
        wrapper.appendChild(list);
        wrapper.appendChild(select);
        select.classList.add("searchable-select-native");
        select.setAttribute("aria-hidden", "true");
        select.tabIndex = -1;
        let searchMode = false;

        const optionText = function (option) {
            return (option.textContent || "").replace(/\s+/g, " ").trim();
        };

        const syncInput = function () {
            const selected = select.selectedOptions && select.selectedOptions.length > 0 ? select.selectedOptions[0] : null;
            input.value = selected ? optionText(selected) : "";
        };

        const close = function (restoreValue) {
            wrapper.classList.remove("is-open");
            input.setAttribute("aria-expanded", "false");
            if (restoreValue) {
                searchMode = false;
                syncInput();
            }
        };

        const open = function () {
            wrapper.classList.add("is-open");
            input.setAttribute("aria-expanded", "true");
        };

        const choose = function (value) {
            select.value = value;
            searchMode = false;
            syncInput();
            select.dispatchEvent(new Event("change", { bubbles: true }));
            close(false);
        };

        const render = function (searchText) {
            const search = normalizeNumericInput(searchText || "").trim().toLowerCase();
            list.innerHTML = "";
            const options = Array.from(select.options)
                .filter(function (option) {
                    return !option.hidden && !option.disabled;
                })
                .filter(function (option) {
                    const text = normalizeNumericInput(optionText(option)).toLowerCase();
                    return !search || text.includes(search);
                })
                .slice(0, 80);

            if (options.length === 0) {
                const empty = document.createElement("div");
                empty.className = "searchable-select-empty";
                empty.textContent = "موردی پیدا نشد";
                list.appendChild(empty);
                return;
            }

            options.forEach(function (option) {
                const item = document.createElement("button");
                item.type = "button";
                item.className = "searchable-select-option";
                item.setAttribute("role", "option");
                item.setAttribute("aria-selected", option.selected ? "true" : "false");
                item.dataset.value = option.value;
                item.textContent = optionText(option);
                item.addEventListener("mousedown", function (event) {
                    event.preventDefault();
                    choose(option.value);
                });
                item.addEventListener("click", function () {
                    choose(option.value);
                });
                item.addEventListener("keydown", function (event) {
                    if (event.key === "Enter" || event.key === " ") {
                        choose(option.value);
                        event.preventDefault();
                    }
                });
                list.appendChild(item);
            });
        };

        const openFullList = function () {
            searchMode = false;
            syncInput();
            render("");
            open();
            input.select();
        };

        input.addEventListener("focus", function () {
            openFullList();
        });

        input.addEventListener("click", function () {
            openFullList();
        });

        input.addEventListener("input", function () {
            searchMode = true;
            render(input.value);
            open();
        });

        input.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                close(true);
                event.preventDefault();
            } else if (event.key === "ArrowDown") {
                render(searchMode ? input.value : "");
                open();
                const items = Array.from(list.querySelectorAll(".searchable-select-option"));
                const currentIndex = items.findIndex(function (item) { return item === document.activeElement; });
                (items[currentIndex + 1] || items[0] || input).focus();
                event.preventDefault();
            } else if (event.key === "Enter") {
                const items = Array.from(list.querySelectorAll(".searchable-select-option"));
                const first = items[0];
                if (first && wrapper.classList.contains("is-open")) {
                    choose(first.dataset.value || "");
                    event.preventDefault();
                }
            }
        });

        list.addEventListener("keydown", function (event) {
            const items = Array.from(list.querySelectorAll(".searchable-select-option"));
            const currentIndex = items.findIndex(function (item) { return item === document.activeElement; });
            if (event.key === "ArrowDown") {
                (items[currentIndex + 1] || items[0] || input).focus();
                event.preventDefault();
            } else if (event.key === "ArrowUp") {
                (items[currentIndex - 1] || input).focus();
                event.preventDefault();
            } else if (event.key === "Escape") {
                input.focus();
                close(true);
                event.preventDefault();
            }
        });

        select.addEventListener("change", function () {
            searchMode = false;
            syncInput();
            render("");
        });

        select.addEventListener("randevoo:refresh-searchable-select", function () {
            searchMode = false;
            syncInput();
            render("");
        });

        wrapper.addEventListener("focusout", function () {
            window.setTimeout(function () {
                if (!wrapper.contains(document.activeElement)) {
                    close(true);
                }
            }, 80);
        });

        syncInput();
        render("");
    };

    document.querySelectorAll("select[data-searchable-select='true']").forEach(initSearchableSelect);

    document.querySelectorAll("[data-location-edit-country='true']").forEach(function (button) {
        button.addEventListener("click", function () {
            const map = {
                "CountryInput_Id": "id",
                "CountryInput_Name": "name",
                "CountryInput_Code": "code",
                "CountryInput_DisplayOrder": "displayOrder"
            };
            Object.keys(map).forEach(function (id) {
                const input = document.getElementById(id);
                if (input) {
                    input.value = button.dataset[map[id]] || "";
                    input.dispatchEvent(new Event("change", { bubbles: true }));
                }
            });
            const active = document.getElementById("CountryInput_IsActive");
            if (active) {
                active.checked = button.dataset.isActive === "true";
            }
            document.getElementById("CountryInput_Name")?.focus();
        });
    });

    document.querySelectorAll("[data-location-edit-city='true']").forEach(function (button) {
        button.addEventListener("click", function () {
            const map = {
                "CityInput_Id": "id",
                "CityInput_CountryId": "countryId",
                "CityInput_Name": "name",
                "CityInput_Latitude": "latitude",
                "CityInput_Longitude": "longitude",
                "CityInput_DisplayOrder": "displayOrder"
            };
            Object.keys(map).forEach(function (id) {
                const input = document.getElementById(id);
                if (input) {
                    input.value = button.dataset[map[id]] || "";
                    input.dispatchEvent(new Event("change", { bubbles: true }));
                    input.dispatchEvent(new Event("randevoo:refresh-searchable-select"));
                }
            });
            const active = document.getElementById("CityInput_IsActive");
            if (active) {
                active.checked = button.dataset.isActive === "true";
            }
            document.getElementById("CityInput_Name")?.focus();
        });
    });

    document.querySelectorAll("[data-event-wizard-form='true']").forEach(function (form) {
        const panels = Array.from(form.querySelectorAll("[data-event-wizard-panel]"));
        const steps = Array.from(form.querySelectorAll("[data-event-wizard-target]"));
        const prevButton = form.querySelector("[data-event-wizard-prev='true']");
        const nextButton = form.querySelector("[data-event-wizard-next='true']");
        const submitButtons = Array.from(form.querySelectorAll("[data-event-wizard-submit='true']"));
        const submitAction = form.querySelector("[data-event-submit-action]");
        let activeIndex = 0;

        if (panels.length === 0 || steps.length === 0) {
            return;
        }

        const hasPanelErrors = function (panel) {
            return Boolean(panel.querySelector(".input-validation-error"))
                || Array.from(panel.querySelectorAll(".field-validation-error, .text-danger")).some(function (element) {
                    return (element.textContent || "").trim().length > 0;
                });
        };

        const reviewSummary = form.querySelector("[data-event-review-summary='true']");
        const reviewValues = reviewSummary
            ? Array.from(reviewSummary.querySelectorAll("[data-review-value]")).reduce(function (values, element) {
                values[element.dataset.reviewValue] = element;
                return values;
            }, {})
            : null;

        const readControl = function (selector) {
            const control = form.querySelector(selector);
            return control ? (control.value || "").trim() : "";
        };

        const selectedText = function (selector) {
            const select = form.querySelector(selector);
            if (!select || !select.selectedOptions || select.selectedOptions.length === 0) {
                return readControl(selector);
            }

            return (select.selectedOptions[0].textContent || "").trim();
        };

        const checkedRadioLabel = function (name) {
            const checked = form.querySelector("input[name='" + name + "']:checked");
            if (!checked) {
                return "";
            }

            if (checked.labels && checked.labels.length > 0) {
                return (checked.labels[0].textContent || "").trim();
            }

            return checked.value || "";
        };

        const setReviewValue = function (key, value) {
            if (!reviewValues || !reviewValues[key]) {
                return;
            }

            reviewValues[key].textContent = (value || "").trim() || "ثبت نشده";
        };

        const setFinancialValue = function (key, value) {
            form.querySelectorAll("[data-financial-value='" + key + "']").forEach(function (element) {
                element.textContent = (value || "").trim() || "0";
            });
        };

        const stripHtml = function (value) {
            const element = document.createElement("div");
            element.innerHTML = value || "";
            return (element.textContent || element.innerText || "").replace(/\s+/g, " ").trim();
        };

        const jalaliToGregorian = function (jy, jm, jd) {
            jy = Number(jy) - 979;
            jm = Number(jm) - 1;
            jd = Number(jd) - 1;
            let jDayNo = 365 * jy + Math.floor(jy / 33) * 8 + Math.floor(((jy % 33) + 3) / 4);
            for (let i = 0; i < jm; ++i) {
                jDayNo += i < 6 ? 31 : 30;
            }
            jDayNo += jd;
            let gDayNo = jDayNo + 79;
            let gy = 1600 + 400 * Math.floor(gDayNo / 146097);
            gDayNo %= 146097;
            let leap = true;
            if (gDayNo >= 36525) {
                gDayNo--;
                gy += 100 * Math.floor(gDayNo / 36524);
                gDayNo %= 36524;
                if (gDayNo >= 365) {
                    gDayNo++;
                } else {
                    leap = false;
                }
            }
            gy += 4 * Math.floor(gDayNo / 1461);
            gDayNo %= 1461;
            if (gDayNo >= 366) {
                leap = false;
                gDayNo--;
                gy += Math.floor(gDayNo / 365);
                gDayNo %= 365;
            }
            const gdMonthDays = [31, leap ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
            let gm = 0;
            while (gm < 12 && gDayNo >= gdMonthDays[gm]) {
                gDayNo -= gdMonthDays[gm];
                gm++;
            }
            return { year: gy, month: gm + 1, day: gDayNo + 1 };
        };

        const gregorianToJalali = function (gy, gm, gd) {
            const gDaysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
            const jDaysInMonth = [31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29];
            gy -= 1600;
            gm -= 1;
            gd -= 1;

            let gDayNo = 365 * gy + Math.floor((gy + 3) / 4) - Math.floor((gy + 99) / 100) + Math.floor((gy + 399) / 400);
            for (let i = 0; i < gm; ++i) {
                gDayNo += gDaysInMonth[i];
            }
            if (gm > 1 && ((gy + 1600) % 4 === 0 && ((gy + 1600) % 100 !== 0 || (gy + 1600) % 400 === 0))) {
                gDayNo++;
            }
            gDayNo += gd;

            let jDayNo = gDayNo - 79;
            const jNp = Math.floor(jDayNo / 12053);
            jDayNo %= 12053;
            let jy = 979 + 33 * jNp + 4 * Math.floor(jDayNo / 1461);
            jDayNo %= 1461;
            if (jDayNo >= 366) {
                jy += Math.floor((jDayNo - 1) / 365);
                jDayNo = (jDayNo - 1) % 365;
            }

            let jm = 0;
            while (jm < 11 && jDayNo >= jDaysInMonth[jm]) {
                jDayNo -= jDaysInMonth[jm];
                jm++;
            }

            return { year: jy, month: jm + 1, day: jDayNo + 1 };
        };

        const pad2 = function (value) {
            return String(value).padStart(2, "0");
        };

        const toPersianDigits = function (value) {
            return String(value)
                .replace(/0/g, "۰")
                .replace(/1/g, "۱")
                .replace(/2/g, "۲")
                .replace(/3/g, "۳")
                .replace(/4/g, "۴")
                .replace(/5/g, "۵")
                .replace(/6/g, "۶")
                .replace(/7/g, "۷")
                .replace(/8/g, "۸")
                .replace(/9/g, "۹");
        };

        const parseDateParts = function (dateText) {
            const normalized = normalizeNumericInput(dateText || "").trim();
            const parts = normalized.split(/[/-]/).map(function (part) {
                return Number.parseInt(part, 10);
            });
            if (parts.length !== 3 || parts.some(function (part) { return !Number.isFinite(part); })) {
                return null;
            }

            if (window.randevooEventEdit && window.randevooEventEdit.isRtl) {
                return jalaliToGregorian(parts[0], parts[1], parts[2]);
            }

            return { year: parts[0], month: parts[1], day: parts[2] };
        };

        const parseDateTime = function (dateSelector, timeSelector) {
            const dateParts = parseDateParts(readControl(dateSelector));
            const timeParts = normalizeNumericInput(readControl(timeSelector)).split(":").map(function (part) {
                return Number.parseInt(part, 10);
            });
            if (!dateParts || timeParts.length !== 2 || timeParts.some(function (part) { return !Number.isFinite(part); })) {
                return null;
            }

            return new Date(dateParts.year, dateParts.month - 1, dateParts.day, timeParts[0], timeParts[1], 0, 0);
        };

        const formatDateForInput = function (date) {
            if (window.randevooEventEdit && window.randevooEventEdit.isRtl) {
                const jalali = gregorianToJalali(date.getFullYear(), date.getMonth() + 1, date.getDate());
                return toPersianDigits(jalali.year + "/" + pad2(jalali.month) + "/" + pad2(jalali.day));
            }

            return date.getFullYear() + "-" + pad2(date.getMonth() + 1) + "-" + pad2(date.getDate());
        };

        const formatTimeForInput = function (date) {
            return pad2(date.getHours()) + ":" + pad2(date.getMinutes());
        };

        const setDateTimeControls = function (dateSelector, timeSelector, date) {
            const dateInput = form.querySelector(dateSelector);
            const timeInput = form.querySelector(timeSelector);
            if (dateInput) {
                dateInput.value = formatDateForInput(date);
            }
            if (timeInput) {
                timeInput.value = formatTimeForInput(date);
            }
        };

        const formatDurationText = function (milliseconds) {
            const totalMinutes = Math.round(milliseconds / 60000);
            const days = Math.floor(totalMinutes / 1440);
            const hours = Math.floor((totalMinutes % 1440) / 60);
            const minutes = totalMinutes % 60;
            const parts = [];
            if (days > 0) {
                parts.push(days.toLocaleString("fa-IR") + " روز");
            }
            if (hours > 0) {
                parts.push(hours.toLocaleString("fa-IR") + " ساعت");
            }
            if (minutes > 0 || parts.length === 0) {
                parts.push(minutes.toLocaleString("fa-IR") + " دقیقه");
            }
            return parts.join(" و ");
        };

        let lastValidDurationMs = 60 * 60 * 1000;

        const syncEventDuration = function () {
            const panel = form.querySelector("[data-event-duration-panel='true']");
            const target = form.querySelector("[data-event-duration-value='true']");
            const start = parseDateTime("#StartDateText", "#StartTimeText");
            let end = parseDateTime("#EndDateText", "#EndTimeText");
            let text = "برای محاسبه مدت، شروع و پایان را کامل کنید.";
            let wasAdjusted = false;

            if (start && end) {
                let diff = end.getTime() - start.getTime();
                if (diff <= 0) {
                    end = new Date(start.getTime() + Math.max(lastValidDurationMs, 60 * 60 * 1000));
                    setDateTimeControls("#EndDateText", "#EndTimeText", end);
                    diff = end.getTime() - start.getTime();
                    wasAdjusted = true;
                }

                lastValidDurationMs = diff;
                text = formatDurationText(diff);
            }

            if (target) {
                target.textContent = text;
            }
            if (panel) {
                panel.classList.remove("has-error");
                panel.classList.toggle("was-adjusted", wasAdjusted);
            }
            return text;
        };

        const readDecimal = function (selector) {
            const value = normalizeNumericInput(readControl(selector)).replace(/[^\d.-]/g, "");
            const parsed = Number.parseFloat(value);
            return Number.isFinite(parsed) ? parsed : 0;
        };

        const readInteger = function (selector) {
            const value = normalizeNumericInput(readControl(selector)).replace(/[^\d-]/g, "");
            const parsed = Number.parseInt(value, 10);
            return Number.isFinite(parsed) ? parsed : 0;
        };

        const formatMoney = function (amount) {
            const currencyText = currentEventCurrencyText();
            const rounded = Math.round((amount + Number.EPSILON) * 100) / 100;
            const numberText = rounded.toLocaleString("fa-IR", {
                maximumFractionDigits: rounded % 1 === 0 ? 0 : 2
            });
            return [numberText, currencyText].filter(Boolean).join(" ");
        };

        const currentEventCurrencyCode = function () {
            return currencySource ? (currencySource.value || "IRR").trim().toUpperCase() : "IRR";
        };

        const currentExchangeRateToIrr = function () {
            const config = window.randevooEventEdit || {};
            const rates = config.currencyRates || {};
            const rateInfo = rates[currentEventCurrencyCode()];
            const rate = rateInfo ? Number.parseFloat(rateInfo.rateToIrr || rateInfo.RateToIrr || 1) : 1;
            return Number.isFinite(rate) && rate > 0 ? rate : 1;
        };

        const formatReportingMoney = function (amount) {
            const converted = Math.round(amount * currentExchangeRateToIrr());
            return converted.toLocaleString("fa-IR", { maximumFractionDigits: 0 }) + " ریال ایران";
        };

        const paymentMethodValue = function () {
            const checked = form.querySelector("input[name='Input.PaymentCollectionMethod']:checked");
            return checked ? checked.value : "";
        };

        const paymentMethodLabel = function () {
            const checked = form.querySelector("input[name='Input.PaymentCollectionMethod']:checked");
            if (!checked) {
                return "";
            }

            const option = checked.closest(".payment-method-option");
            const title = option ? option.querySelector("strong") : null;
            return title ? (title.textContent || "").trim() : checkedRadioLabel("Input.PaymentCollectionMethod");
        };

        const isOrganizerManualTransfer = function () {
            return paymentMethodValue().includes("OrganizerManualTransfer") || paymentMethodValue() === "2";
        };

        const syncPaymentMethodFields = function () {
            const panel = form.querySelector("[data-organizer-payment-instructions='true']");
            if (!panel) {
                return;
            }

            const accountSelect = panel.querySelector("[data-organizer-payment-account-select='true']");
            const isVisible = isOrganizerManualTransfer();
            panel.classList.toggle("d-none", !isVisible);
            if (accountSelect) {
                accountSelect.disabled = !isVisible;
                accountSelect.required = isVisible;
            }
        };

        const paymentAccountLabel = function () {
            const select = form.querySelector("[data-organizer-payment-account-select='true']");
            return select ? selectedText("#" + select.id) : "";
        };

        const syncOrganizerPaymentAccounts = function () {
            const select = form.querySelector("[data-organizer-payment-account-select='true']");
            if (!select) {
                return;
            }

            const config = window.randevooEventEdit || {};
            const accounts = Array.isArray(config.organizerPaymentAccounts) ? config.organizerPaymentAccounts : [];
            const currencyCode = currentEventCurrencyCode();
            const selectedValue = select.value;
            const warning = form.querySelector("[data-organizer-payment-warning='true']");
            const matchingAccounts = accounts.filter(function (account) {
                const isActive = account && (account.isActive !== undefined ? account.isActive : account.IsActive);
                return account && isActive !== false && String(account.currencyCode || account.CurrencyCode || "").toUpperCase() === currencyCode;
            });

            select.innerHTML = '<option value="">انتخاب حساب</option>';
            matchingAccounts.forEach(function (account) {
                const option = document.createElement("option");
                option.value = String(account.id || account.Id || "");
                option.textContent = account.label || account.Label || option.value;
                select.appendChild(option);
            });

            if (selectedValue && matchingAccounts.some(function (account) { return String(account.id || account.Id) === selectedValue; })) {
                select.value = selectedValue;
            } else if (selectedValue) {
                select.value = "";
            }

            if (warning) {
                const hasWarning = matchingAccounts.length === 0;
                warning.classList.toggle("d-none", !hasWarning);
                warning.textContent = hasWarning
                    ? "حساب فعال " + currencyCode + " برای این برگزارکننده ثبت نشده است. ابتدا حساب را در پروفایل برگزارکننده ثبت و فعال کنید."
                    : "";
            }

            select.dispatchEvent(new Event("randevoo:refresh-searchable-select"));
        };

        const syncFinancialPreview = function () {
            const malePrice = readDecimal("#Input_MaleTicketPrice");
            const femalePrice = readDecimal("#Input_FemaleTicketPrice");
            const maleCapacity = Math.max(0, readInteger("#Input_CapacityMale"));
            const femaleCapacity = Math.max(0, readInteger("#Input_CapacityFemale"));
            const commissionRate = Math.min(100, Math.max(0, readDecimal("#Input_OrganizerCommissionPercent")));
            const grossTotal = (malePrice * maleCapacity) + (femalePrice * femaleCapacity);
            const commissionTotal = grossTotal * commissionRate / 100;
            const organizerNetTotal = grossTotal - commissionTotal;
            const organizerCollects = isOrganizerManualTransfer();

            setFinancialValue("gross-total", formatMoney(grossTotal));
            setFinancialValue("platform-commission-total", formatMoney(commissionTotal) + " (" + commissionRate.toLocaleString("fa-IR") + "%)");
            setFinancialValue("organizer-net-total", formatMoney(organizerNetTotal));
            setFinancialValue(
                "settlement-effect",
                organizerCollects
                    ? "بدهی برگزارکننده: " + formatMoney(commissionTotal)
                    : "قابل برداشت برای برگزارکننده: " + formatMoney(organizerNetTotal));
            setFinancialValue(
                "settlement-note",
                organizerCollects
                    ? "در این روش پول اول نزد برگزارکننده است؛ بعد از تایید پرداخت، فقط کمیسیون پلتفرم از حساب او کم می‌شود."
                    : "در این روش پول اول نزد پلتفرم است؛ بعد از تایید پرداخت، سهم خالص برگزارکننده به حساب او اضافه می‌شود.");
            setFinancialValue("gross-total-irr", formatReportingMoney(grossTotal));
            setFinancialValue("platform-commission-total-irr", formatReportingMoney(commissionTotal));
            setFinancialValue("organizer-net-total-irr", formatReportingMoney(organizerNetTotal));
        };

        const currencySource = form.querySelector("[data-event-currency-source='true']");
        const currencyTargets = Array.from(form.querySelectorAll("[data-event-currency-target='true']"));
        const currencyDisplays = Array.from(form.querySelectorAll("[data-event-currency-display='true']"));

        const currentEventCurrencyText = function () {
            if (!currencySource || !currencySource.selectedOptions || currencySource.selectedOptions.length === 0) {
                const config = window.randevooEventEdit || {};
                const rates = config.currencyRates || {};
                const currencyCode = currencySource ? currencySource.value : "";
                const rateInfo = rates[currencyCode];
                return rateInfo && (rateInfo.displayNameFa || rateInfo.DisplayNameFa)
                    ? (rateInfo.displayNameFa || rateInfo.DisplayNameFa) + " (" + currencyCode + ")"
                    : currencyCode;
            }

            return (currencySource.selectedOptions[0].textContent || "").trim();
        };

        const syncEventCurrency = function () {
            if (!currencySource) {
                return;
            }

            const currencyCode = currencySource.value;
            const currencyText = currentEventCurrencyText();

            if (!currencyCode) {
                return;
            }

            currencyTargets.forEach(function (control) {
                control.value = currencyCode;
            });
            currencyDisplays.forEach(function (control) {
                control.value = currencyText || currencyCode;
            });
            syncOrganizerPaymentAccounts();
        };

        const syncEventReview = function () {
            if (!reviewValues) {
                return;
            }

            syncEventCurrency();
            const eventModeId = readControl("input[name='Input.EventModeId']:checked");
            const isOnline = window.randevooEventEdit && eventModeId === window.randevooEventEdit.onlineModeId;
            const start = [readControl("#StartDateText"), readControl("#StartTimeText")].filter(Boolean).join(" ");
            const end = [readControl("#EndDateText"), readControl("#EndTimeText")].filter(Boolean).join(" ");
            const duration = syncEventDuration();
            const countryCity = [readControl("#Input_Country"), readControl("#Input_City"), readControl("#Input_Region")].filter(Boolean).join("، ");
            const venue = readControl("#Input_VenueName");
            const platform = selectedText("#Input_OnlineEventPlatformId");
            const joinUrl = readControl("#Input_OnlineJoinUrl");
            const onlineInstructions = readControl("#Input_OnlineAccessInstructions");
            const organizerPayment = paymentAccountLabel();
            const eventCurrencyText = currentEventCurrencyText();
            const maleTicket = [readControl("#Input_MaleTicketPrice"), eventCurrencyText].filter(Boolean).join(" ");
            const femaleTicket = [readControl("#Input_FemaleTicketPrice"), eventCurrencyText].filter(Boolean).join(" ");
            const selectedTags = Array.from(form.querySelectorAll("#Input_TagIds option:checked"))
                .map(function (option) { return (option.textContent || "").trim(); })
                .filter(Boolean);
            const imageCount = ["1", "2", "3"].filter(function (index) {
                const hidden = form.querySelector("#Input_Image" + index);
                const file = form.querySelector("#Image" + index + "File");
                return Boolean((hidden && hidden.value) || (file && file.files && file.files.length > 0));
            }).length;

            setReviewValue("title", readControl("#Input_Title"));
            setReviewValue("planner", selectedText("#AssignedPlannerId"));
            setReviewValue("event-type", selectedText("#Input_EventTypeId"));
            setReviewValue("event-mode", checkedRadioLabel("Input.EventModeId"));
            setReviewValue("delivery-detail", isOnline ? [platform, joinUrl].filter(Boolean).join(" / ") : [countryCity, venue].filter(Boolean).join(" / "));
            setReviewValue("address", isOnline ? joinUrl : readControl("#Input_Address"));
            setReviewValue("start", start);
            setReviewValue("end", end);
            setReviewValue("duration", duration);
            setReviewValue("male-ticket", maleTicket);
            setReviewValue("female-ticket", femaleTicket);
            setReviewValue("capacity", [readControl("#Input_CapacityMale") || "0", readControl("#Input_CapacityFemale") || "0"].join(" آقا / ") + " خانم");
            setReviewValue("age-range", [selectedText("#Input_AgeRangeForMale"), selectedText("#Input_AgeRangeForFemale")].filter(Boolean).join(" آقا / ") + (selectedText("#Input_AgeRangeForFemale") ? " خانم" : ""));
            setReviewValue("education", selectedText("#Input_MinimumEducationLevelId"));
            setReviewValue("like-limit", readControl("#Input_LikeLimit"));
            setReviewValue("tags", selectedTags.join("، "));
            setReviewValue("images", imageCount + " / 3");
            setReviewValue("payment-method", paymentMethodLabel());
            setReviewValue("organizer-payment", organizerPayment);
            setReviewValue("online-instructions", onlineInstructions);
            setReviewValue("description", stripHtml(readControl("#Input_DescriptionHtml")));
            syncPaymentMethodFields();
            syncFinancialPreview();
        };

        const markStepErrors = function () {
            steps.forEach(function (step) {
                const key = step.dataset.eventWizardTarget;
                const panel = panels.find(function (candidate) {
                    return candidate.dataset.eventWizardPanel === key;
                });
                step.classList.toggle("has-error", Boolean(panel && hasPanelErrors(panel)));
            });
        };

        const setActiveStep = function (index) {
            syncEventReview();
            activeIndex = Math.max(0, Math.min(index, panels.length - 1));
            const activePanel = panels[activeIndex];
            const activeKey = activePanel.dataset.eventWizardPanel;

            panels.forEach(function (panel, panelIndex) {
                const isActive = panelIndex === activeIndex;
                panel.classList.toggle("is-active", isActive);
                panel.setAttribute("aria-hidden", isActive ? "false" : "true");
            });

            steps.forEach(function (step) {
                const isActive = step.dataset.eventWizardTarget === activeKey;
                step.classList.toggle("is-active", isActive);
                if (isActive) {
                    step.setAttribute("aria-current", "step");
                    step.scrollIntoView({ behavior: "smooth", inline: "nearest", block: "nearest" });
                } else {
                    step.removeAttribute("aria-current");
                }
            });

            if (prevButton) {
                prevButton.disabled = activeIndex === 0;
            }

            if (nextButton) {
                nextButton.classList.toggle("d-none", activeIndex === panels.length - 1);
            }

            submitButtons.forEach(function (button) {
                button.classList.toggle("d-none", activeIndex !== panels.length - 1);
            });

            window.setTimeout(function () {
                window.dispatchEvent(new Event("resize"));
            }, 80);
        };

        steps.forEach(function (step, index) {
            step.addEventListener("click", function () {
                markStepErrors();
                setActiveStep(index);
            });
        });

        if (prevButton) {
            prevButton.addEventListener("click", function () {
                setActiveStep(activeIndex - 1);
            });
        }

        if (nextButton) {
            nextButton.addEventListener("click", function () {
                markStepErrors();
                setActiveStep(activeIndex + 1);
            });
        }

        submitButtons.forEach(function (button) {
            button.addEventListener("click", function () {
                if (submitAction) {
                    submitAction.value = button.dataset.submitMode || "draft";
                }
            });
        });

        form.addEventListener("input", syncEventReview);
        form.addEventListener("change", function () {
            syncEventReview();
        });
        form.addEventListener("click", function (event) {
            if (event.target.closest(".image-clear-button")) {
                window.setTimeout(syncEventReview, 0);
            }
            if (event.target.closest("[data-financial-calc-button='true']")) {
                syncEventReview();
            }
            if (event.target.closest("[data-event-review-print='true']")) {
                syncEventReview();
                window.print();
            }
        });

        form.addEventListener("submit", function () {
            window.setTimeout(function () {
                markStepErrors();
                const firstInvalidIndex = panels.findIndex(hasPanelErrors);
                if (firstInvalidIndex >= 0) {
                    setActiveStep(firstInvalidIndex);
                }
            }, 0);
        });

        markStepErrors();
        syncEventCurrency();
        syncPaymentMethodFields();
        syncFinancialPreview();
        const firstInvalidIndex = panels.findIndex(hasPanelErrors);
        setActiveStep(firstInvalidIndex >= 0 ? firstInvalidIndex : 0);
    });

    document.querySelectorAll("[data-discount-type-group='true']").forEach(function (group) {
        const form = group.closest("form");
        const hiddenValue = form ? form.querySelector("[data-discount-value-hidden='true']") : null;
        const fixedPanel = form ? form.querySelector("[data-discount-value-panel='fixed']") : null;
        const percentagePanel = form ? form.querySelector("[data-discount-value-panel='percentage']") : null;
        const fixedInput = form ? form.querySelector("[data-discount-value-source='fixed']") : null;
        const percentageInput = form ? form.querySelector("[data-discount-value-source='percentage']") : null;
        const radios = group.querySelectorAll("input[type='radio']");

        const selectedType = function () {
            const selected = Array.from(radios).find(function (radio) { return radio.checked; });
            return selected ? selected.value : "";
        };

        const syncDiscountValue = function () {
            if (!hiddenValue) {
                return;
            }

            const type = selectedType();
            const isFixedType = type.includes("FixedAmount") || type === "1";
            hiddenValue.value = isFixedType
                ? normalizeNumericInput(fixedInput ? fixedInput.value : hiddenValue.value)
                : normalizeNumericInput(percentageInput ? percentageInput.value : hiddenValue.value);
        };

        const syncPanels = function () {
            const type = selectedType();
            const isFixed = type.includes("FixedAmount") || type === "1";
            if (fixedPanel) {
                fixedPanel.classList.toggle("d-none", !isFixed);
            }
            if (percentagePanel) {
                percentagePanel.classList.toggle("d-none", isFixed);
            }
            if (fixedInput) {
                fixedInput.disabled = !isFixed;
            }
            if (percentageInput) {
                percentageInput.disabled = isFixed;
            }
            syncDiscountValue();
        };

        radios.forEach(function (radio) {
            radio.addEventListener("change", syncPanels);
        });
        [fixedInput, percentageInput].forEach(function (input) {
            if (input) {
                input.addEventListener("input", syncDiscountValue);
            }
        });
        if (form) {
            form.addEventListener("submit", syncDiscountValue);
        }
        syncPanels();
    });

    if (window.randevooEventEdit && window.L) {
        const config = window.randevooEventEdit;
        const countrySelector = config.countrySelector ? document.querySelector(config.countrySelector) : null;
        const latitudeInput = document.querySelector(config.latitudeSelector);
        const longitudeInput = document.querySelector(config.longitudeSelector);
        const latitudeDisplay = config.latitudeDisplaySelector ? document.querySelector(config.latitudeDisplaySelector) : null;
        const longitudeDisplay = config.longitudeDisplaySelector ? document.querySelector(config.longitudeDisplaySelector) : null;
        const citySelector = document.querySelector(config.citySelector);
        const mapElement = document.getElementById("eventMap");
        const eventModeControls = config.eventModeSelector ? Array.from(document.querySelectorAll(config.eventModeSelector)) : [];
        const onlineModeId = config.onlineModeId || "1";
        const locationSections = document.querySelectorAll("[data-location-section='true']");
        const onlineSections = document.querySelectorAll("[data-online-section='true']");
        let eventMap = null;

        const syncDeliverySections = function () {
            const selectedMode = eventModeControls.length > 1
                ? eventModeControls.find(function (item) { return item.checked; })
                : eventModeControls[0];
            const isOnline = selectedMode && selectedMode.value === onlineModeId;
            locationSections.forEach(function (section) {
                section.classList.toggle("d-none", isOnline);
                section.querySelectorAll("input, select, textarea").forEach(function (control) {
                    control.disabled = isOnline;
                });
            });
            onlineSections.forEach(function (section) {
                section.classList.toggle("d-none", !isOnline);
                section.querySelectorAll("input, select, textarea").forEach(function (control) {
                    control.disabled = !isOnline;
                });
            });

            if (!isOnline && eventMap) {
                window.setTimeout(function () {
                    eventMap.invalidateSize();
                }, 50);
            }
        };

        if (eventModeControls.length > 0) {
            eventModeControls.forEach(function (control) {
                control.addEventListener("change", syncDeliverySections);
            });
            syncDeliverySections();
        }

        if (mapElement && latitudeInput && longitudeInput && citySelector) {
            const cityOptions = Array.isArray(config.cityOptions) ? config.cityOptions : [];

            const findSelectedCity = function () {
                return cityOptions.find(function (city) {
                    return city.name === citySelector.value || city.Name === citySelector.value;
                });
            };

            const getCityName = function (city) {
                return city.name || city.Name;
            };

            const getCountryName = function (city) {
                return city.countryName || city.CountryName;
            };

            const getLatitude = function (city) {
                return parseFloat(city.latitude ?? city.Latitude);
            };

            const getLongitude = function (city) {
                return parseFloat(city.longitude ?? city.Longitude);
            };

            const renderCityOptions = function () {
                if (!countrySelector || cityOptions.length === 0) {
                    return;
                }

                const selectedCountry = countrySelector.value;
                const previousCity = citySelector.value;
                const filteredCities = cityOptions.filter(function (city) {
                    return getCountryName(city) === selectedCountry;
                });

                citySelector.innerHTML = "";
                filteredCities.forEach(function (city) {
                    const option = document.createElement("option");
                    option.value = getCityName(city);
                    const isActive = city.isActive ?? city.IsActive;
                    option.textContent = isActive === false ? getCityName(city) + " (غیرفعال)" : getCityName(city);
                    citySelector.appendChild(option);
                });

                const hasPrevious = filteredCities.some(function (city) {
                    return getCityName(city) === previousCity;
                });
                citySelector.value = hasPrevious ? previousCity : (filteredCities[0] ? getCityName(filteredCities[0]) : "");
                citySelector.dispatchEvent(new Event("randevoo:refresh-searchable-select"));
            };

            const startLat = parseFloat(latitudeInput.value || "35.7219");
            const startLng = parseFloat(longitudeInput.value || "51.3347");
            const map = L.map(mapElement, { scrollWheelZoom: false }).setView([startLat, startLng], 13);
            eventMap = map;

            L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
                maxZoom: 19,
                attribution: "&copy; OpenStreetMap"
            }).addTo(map);

            const marker = L.marker([startLat, startLng], { draggable: true }).addTo(map);

            function toPersianCoordinateDigits(value) {
                return String(value)
                    .replace(/0/g, "۰")
                    .replace(/1/g, "۱")
                    .replace(/2/g, "۲")
                    .replace(/3/g, "۳")
                    .replace(/4/g, "۴")
                    .replace(/5/g, "۵")
                    .replace(/6/g, "۶")
                    .replace(/7/g, "۷")
                    .replace(/8/g, "۸")
                    .replace(/9/g, "۹");
            }

            function syncCoordinates(lat, lng) {
                latitudeInput.value = lat.toFixed(4);
                longitudeInput.value = lng.toFixed(4);

                if (latitudeDisplay) {
                    latitudeDisplay.textContent = toPersianCoordinateDigits(lat.toFixed(4));
                }

                if (longitudeDisplay) {
                    longitudeDisplay.textContent = toPersianCoordinateDigits(lng.toFixed(4));
                }
            }

            marker.on("dragend", function (event) {
                const position = event.target.getLatLng();
                syncCoordinates(position.lat, position.lng);
            });

            map.on("click", function (event) {
                marker.setLatLng(event.latlng);
                syncCoordinates(event.latlng.lat, event.latlng.lng);
            });

            citySelector.addEventListener("change", function () {
                const selected = findSelectedCity();
                if (!selected) {
                    return;
                }

                const selectedLat = getLatitude(selected);
                const selectedLng = getLongitude(selected);
                map.setView([selectedLat, selectedLng], 12);
                marker.setLatLng([selectedLat, selectedLng]);
                syncCoordinates(selectedLat, selectedLng);
            });

            if (countrySelector) {
                countrySelector.addEventListener("change", function () {
                    renderCityOptions();
                    citySelector.dispatchEvent(new Event("change"));
                });
            }

            setTimeout(function () {
                map.invalidateSize();
            }, 200);

            renderCityOptions();
            syncCoordinates(startLat, startLng);
        }
    }

    if (document.querySelector(".admin-shell")) {
        const trackingPath = "/activity/track";
        const pageStart = Date.now();
        let heartbeatTimer = null;
        let lastClickAt = 0;

        const resolveModule = function () {
            const segments = window.location.pathname.split("/").filter(Boolean);
            return segments.length === 0 ? "dashboard" : segments[0].toLowerCase();
        };

        const postActivity = function (payload) {
            if (!payload) {
                return;
            }

            const body = JSON.stringify(payload);
            if (navigator.sendBeacon) {
                const blob = new Blob([body], { type: "application/json" });
                navigator.sendBeacon(trackingPath, blob);
                return;
            }

            fetch(trackingPath, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: body,
                keepalive: true,
                credentials: "same-origin"
            }).catch(function () {
            });
        };

        const track = function (type, action, description, metadata) {
            postActivity({
                type: type,
                action: action,
                description: description,
                module: resolveModule(),
                path: window.location.pathname + window.location.search,
                metadata: metadata || undefined
            });
        };

        heartbeatTimer = window.setInterval(function () {
            track("heartbeat", "AdminHeartbeat", "Admin session heartbeat.", {
                title: document.title
            });
        }, 60000);

        document.addEventListener("click", function (event) {
            const target = event.target && event.target.closest ? event.target.closest("a,button,[role='button'],input[type='submit']") : null;
            if (!target) {
                return;
            }

            const now = Date.now();
            if (now - lastClickAt < 1500) {
                return;
            }

            lastClickAt = now;
            const label = (target.innerText || target.getAttribute("aria-label") || target.getAttribute("title") || target.id || target.className || "click")
                .replace(/\s+/g, " ")
                .trim()
                .slice(0, 120);

            track("click", "AdminClick", "Admin UI click.", {
                label: label,
                tagName: target.tagName.toLowerCase()
            });
        }, true);

        const flushTimeSpent = function () {
            if (heartbeatTimer) {
                window.clearInterval(heartbeatTimer);
                heartbeatTimer = null;
            }

            const durationSeconds = Math.max(1, Math.round((Date.now() - pageStart) / 1000));
            postActivity({
                type: "time_spent",
                action: "AdminTimeSpent",
                description: "Tracked dwell time on admin page.",
                module: resolveModule(),
                path: window.location.pathname + window.location.search,
                durationSeconds: durationSeconds,
                metadata: {
                    title: document.title
                }
            });
        };

        window.addEventListener("pagehide", flushTimeSpent, { once: true });
    }

    document.querySelectorAll(".event-status-transition-form").forEach(function (form) {
        const actionInputs = Array.from(form.querySelectorAll("input[name='action']"));
        const impact = form.querySelector("[data-transition-impact]");
        const target = form.querySelector("[data-transition-target]");
        const description = form.querySelector("[data-transition-description]");
        const noteWrapper = form.querySelector("[data-transition-note-wrapper]");
        const noteInput = form.querySelector("[data-transition-note]");
        const noteLabel = form.querySelector("[data-transition-note-label]");
        const confirmation = form.querySelector("[data-transition-confirm]");
        const confirmationLabel = form.querySelector("[data-transition-confirm-label]");
        const submitButton = form.querySelector("[data-transition-submit]");
        const cancellationAction = form.dataset.cancellationAction || "CancelEvent";
        const cancellationPreviewUrl = form.dataset.cancellationPreviewUrl || "";
        const cancellationPreviewPanel = form.querySelector("[data-cancellation-preview]");
        const cancellationPreviewState = form.querySelector("[data-cancellation-preview-state]");
        const cancellationPreviewContent = form.querySelector("[data-cancellation-preview-content]");
        const cancellationSummary = form.querySelector("[data-cancellation-summary]");
        const cancellationMetrics = form.querySelector("[data-cancellation-metrics]");
        const cancellationBlockers = form.querySelector("[data-cancellation-blockers]");
        const cancellationConsequences = form.querySelector("[data-cancellation-consequences]");
        const cancellationWarnings = form.querySelector("[data-cancellation-warnings]");
        const cancellationMessageWrapper = form.querySelector("[data-cancellation-message-wrapper]");
        const cancellationMessageInput = form.querySelector("[data-cancellation-message]");
        let cancellationPreview = null;
        let cancellationPreviewLoading = false;

        const selectedAction = function () {
            return actionInputs.find(function (input) { return input.checked; }) || null;
        };

        const readValue = function (source, camelName, pascalName, fallback) {
            if (!source) {
                return fallback;
            }

            if (Object.prototype.hasOwnProperty.call(source, camelName)) {
                return source[camelName];
            }

            if (Object.prototype.hasOwnProperty.call(source, pascalName)) {
                return source[pascalName];
            }

            return fallback;
        };

        const renderCancellationList = function (wrapper, items) {
            if (!wrapper) {
                return;
            }

            const list = wrapper.querySelector("ul");
            if (!list) {
                return;
            }

            list.innerHTML = "";
            const values = Array.isArray(items) ? items : [];
            wrapper.hidden = values.length === 0;
            values.forEach(function (item) {
                const li = document.createElement("li");
                li.textContent = item;
                list.appendChild(li);
            });
        };

        const renderCancellationPreview = function (preview) {
            if (!cancellationPreviewContent || !preview) {
                return;
            }

            const summary = readValue(preview, "summary", "Summary", "");
            const metrics = readValue(preview, "metrics", "Metrics", []);
            const blockers = readValue(preview, "blockingReasons", "BlockingReasons", []);
            const consequences = readValue(preview, "consequences", "Consequences", []);
            const warnings = readValue(preview, "warnings", "Warnings", []);
            const suggestedMessage = readValue(preview, "suggestedPublicMessage", "SuggestedPublicMessage", "");

            if (cancellationSummary) {
                cancellationSummary.textContent = summary;
            }

            if (cancellationMetrics) {
                cancellationMetrics.innerHTML = "";
                (Array.isArray(metrics) ? metrics : []).forEach(function (metric) {
                    const card = document.createElement("div");
                    card.className = "event-cancellation-metric";

                    const label = document.createElement("span");
                    label.textContent = readValue(metric, "label", "Label", "");
                    card.appendChild(label);

                    const value = document.createElement("strong");
                    value.textContent = readValue(metric, "value", "Value", "");
                    card.appendChild(value);

                    const hintText = readValue(metric, "hint", "Hint", "");
                    if (hintText) {
                        const hint = document.createElement("small");
                        hint.textContent = hintText;
                        card.appendChild(hint);
                    }

                    cancellationMetrics.appendChild(card);
                });
            }

            renderCancellationList(cancellationBlockers, blockers);
            renderCancellationList(cancellationConsequences, consequences);
            renderCancellationList(cancellationWarnings, warnings);

            if (cancellationMessageInput && !cancellationMessageInput.value.trim() && suggestedMessage) {
                cancellationMessageInput.value = suggestedMessage;
            }

            if (cancellationPreviewState) {
                cancellationPreviewState.hidden = true;
            }

            cancellationPreviewContent.hidden = false;
        };

        const renderCancellationError = function (message) {
            cancellationPreview = { canCancel: false, blockingReasons: [message] };

            if (cancellationPreviewState) {
                cancellationPreviewState.hidden = false;
                cancellationPreviewState.innerHTML = "";
                const icon = document.createElement("i");
                icon.className = "bi bi-exclamation-triangle";
                cancellationPreviewState.appendChild(icon);
                const text = document.createElement("span");
                text.textContent = message;
                cancellationPreviewState.appendChild(text);
            }

            if (cancellationPreviewContent) {
                cancellationPreviewContent.hidden = true;
            }
        };

        const loadCancellationPreview = function () {
            if (!cancellationPreviewUrl) {
                renderCancellationError("مسیر بررسی چک‌لیست لغو برای این صفحه تنظیم نشده است.");
                return;
            }

            if (cancellationPreview || cancellationPreviewLoading) {
                return;
            }

            cancellationPreviewLoading = true;
            if (cancellationPreviewPanel) {
                cancellationPreviewPanel.hidden = false;
            }
            if (cancellationPreviewState) {
                cancellationPreviewState.hidden = false;
                cancellationPreviewState.innerHTML = '<span class="spinner-border spinner-border-sm" aria-hidden="true"></span><span>در حال بررسی اثرات لغو این رویداد...</span>';
            }
            if (cancellationPreviewContent) {
                cancellationPreviewContent.hidden = true;
            }

            fetch(cancellationPreviewUrl, {
                method: "GET",
                headers: {
                    "Accept": "application/json"
                },
                credentials: "same-origin"
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("امکان دریافت چک‌لیست لغو وجود ندارد.");
                    }

                    return response.json();
                })
                .then(function (preview) {
                    cancellationPreview = preview;
                    renderCancellationPreview(preview);
                })
                .catch(function (error) {
                    renderCancellationError(error.message || "امکان دریافت چک‌لیست لغو وجود ندارد.");
                })
                .finally(function () {
                    cancellationPreviewLoading = false;
                    updateState();
                });
        };

        const updateState = function () {
            const selected = selectedAction();
            const hasSelection = Boolean(selected);
            const requiresNote = selected?.dataset.requiresNote === "true";
            const hasNote = !requiresNote || Boolean((noteInput?.value || "").trim());
            const confirmed = Boolean(confirmation?.checked);
            const isCancellation = hasSelection && selected.value === cancellationAction;
            const hasCancellationMessage = !isCancellation || Boolean((cancellationMessageInput?.value || "").trim());
            const cancellationCanSubmit = !isCancellation
                || (!cancellationPreviewLoading && Boolean(cancellationPreview) && readValue(cancellationPreview, "canCancel", "CanCancel", false) === true);

            if (impact) {
                impact.hidden = !hasSelection;
            }

            if (target) {
                target.textContent = selected?.dataset.target || "";
            }

            if (description) {
                description.textContent = selected?.dataset.description || "";
            }

            if (noteWrapper) {
                noteWrapper.hidden = !requiresNote;
            }

            if (noteInput) {
                noteInput.required = requiresNote;
                noteInput.placeholder = selected?.dataset.notePlaceholder || "";
            }

            if (noteLabel) {
                noteLabel.textContent = selected?.dataset.noteLabel || "توضیحات";
            }

            if (confirmationLabel) {
                confirmationLabel.textContent = selected?.dataset.confirmation || "تایید می‌کنم این تغییر وضعیت ثبت شود.";
            }

            if (cancellationPreviewPanel) {
                cancellationPreviewPanel.hidden = !isCancellation;
            }

            if (cancellationMessageWrapper) {
                cancellationMessageWrapper.hidden = !isCancellation;
            }

            if (cancellationMessageInput) {
                cancellationMessageInput.required = isCancellation;
            }

            if (isCancellation) {
                loadCancellationPreview();
            }

            if (submitButton) {
                submitButton.disabled = !hasSelection || !hasNote || !confirmed || !hasCancellationMessage || !cancellationCanSubmit;
            }
        };

        actionInputs.forEach(function (input) {
            input.addEventListener("change", updateState);
        });

        noteInput?.addEventListener("input", updateState);
        confirmation?.addEventListener("change", updateState);
        cancellationMessageInput?.addEventListener("input", updateState);
        updateState();
    });
});
