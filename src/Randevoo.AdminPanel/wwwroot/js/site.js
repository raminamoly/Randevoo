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

    document.querySelectorAll("[data-event-wizard-form='true']").forEach(function (form) {
        const panels = Array.from(form.querySelectorAll("[data-event-wizard-panel]"));
        const steps = Array.from(form.querySelectorAll("[data-event-wizard-target]"));
        const prevButton = form.querySelector("[data-event-wizard-prev='true']");
        const nextButton = form.querySelector("[data-event-wizard-next='true']");
        const submitButton = form.querySelector("[data-event-wizard-submit='true']");
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

            if (submitButton) {
                submitButton.classList.toggle("d-none", activeIndex !== panels.length - 1);
            }

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
                    option.textContent = getCityName(city);
                    citySelector.appendChild(option);
                });

                const hasPrevious = filteredCities.some(function (city) {
                    return getCityName(city) === previousCity;
                });
                citySelector.value = hasPrevious ? previousCity : (filteredCities[0] ? getCityName(filteredCities[0]) : "");
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

            function toPersianDigits(value) {
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
                    latitudeDisplay.textContent = toPersianDigits(lat.toFixed(4));
                }

                if (longitudeDisplay) {
                    longitudeDisplay.textContent = toPersianDigits(lng.toFixed(4));
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
});
