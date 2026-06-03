$(function () {
    $(".image-input").on("change", function () {
        const input = this;
        const target = $(input).data("target");
        const file = input.files && input.files[0];

        if (!file || !target) {
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            $(target).attr("src", e.target.result);
        };
        reader.readAsDataURL(file);
    });

    $(".editor-toolbar [data-editor-command]").on("click", function () {
        const command = $(this).data("editor-command");
        const editor = $(".rich-text").get(0);
        if (!editor) {
            return;
        }

        editor.focus();
        const start = editor.selectionStart ?? 0;
        const end = editor.selectionEnd ?? 0;
        const value = editor.value;
        const selected = value.substring(start, end) || "Text";

        const wrapper = command === "bold"
            ? ["<strong>", "</strong>"]
            : command === "italic"
                ? ["<em>", "</em>"]
                : ["<u>", "</u>"];

        editor.value = value.substring(0, start) + wrapper[0] + selected + wrapper[1] + value.substring(end);
    });

    if ($.fn.persianDatepicker && $(".jalali-picker").length) {
        $(".jalali-picker").persianDatepicker({
            format: "YYYY/MM/DD HH:mm",
            initialValue: false,
            observer: true,
            toolbox: {
                calendarSwitch: {
                    enabled: false
                }
            }
        });
    }
});
