(function () {
    const pick = (obj, names) => {
        const k = Object.keys(obj ?? {}).find(k => names.includes(k.toLowerCase()));
        return k ? obj[k] : undefined;
    };

    function summaryBox(form) {
        let box = form.querySelector("[data-form-errors]");
        if (!box) {
            box = document.createElement("div");
            box.className = "alert alert-danger d-none";
            box.setAttribute("role", "alert");
            box.setAttribute("data-form-errors", "");
            form.prepend(box);
        }
        return box;
    }

    function showSummary(form, messages) {
        const box = summaryBox(form);
        box.replaceChildren();
        if (!messages.length) { box.classList.add("d-none"); return; }
        const ul = document.createElement("ul");
        ul.className = "mb-0";
        messages.forEach(m => {
            const li = document.createElement("li");
            li.textContent = m;
            ul.appendChild(li);
        });
        box.appendChild(ul);
        box.classList.remove("d-none");
    }

    function parse(result) {
        const general = [], fields = {};
        const add = (key, value) => {
            const msgs = [].concat(value).filter(Boolean).map(String);
            if (key) (fields[key] ??= []).push(...msgs); else general.push(...msgs);
        };

        const errors = pick(result, ["errors"]);
        if (Array.isArray(errors)) add("", errors);
        else if (errors && typeof errors === "object") Object.entries(errors).forEach(([k, v]) => add(k, v));

        const fe = pick(result, ["fielderrors"]);
        if (fe && typeof fe === "object") Object.entries(fe).forEach(([k, v]) => add(k, v));

        const single = pick(result, ["message", "detail", "title"]);
        if (!general.length && !Object.keys(fields).length && typeof single === "string") general.push(single);

        return { general, fields };
    }

    function applyFieldErrors(form, fields) {
        const toShow = {}, leftover = [];
        for (const [key, msgs] of Object.entries(fields)) {
            const k = CSS.escape(key);
            const control = form.querySelector(`[name="${k}"], [name$=".${k}"]`);
            if (control) toShow[control.name] = msgs.join(" ");
            else leftover.push(...msgs);
        }
        const validator = window.jQuery && $(form).data("validator");
        if (validator && Object.keys(toShow).length) validator.showErrors(toShow);
        else leftover.push(...Object.values(toShow));
        return leftover;
    }

    document.addEventListener("submit", async e => {
        const form = e.target.closest("form[data-ajax-form]");
        if (!form) return;
        e.preventDefault();

        if (window.jQuery && !$(form).valid()) return;
        showSummary(form, []);

        const btn = form.querySelector("[type=submit]");
        if (btn) btn.disabled = true;

        try {
            const res = await fetch(form.action || location.href, { method: "POST", body: new FormData(form) });
            let result = null;
            try { result = await res.json(); } catch { }

            if (!result) {
                showSummary(form, [`Unexpected server response (${res.status}). Please try again.`]);
                return;
            }

            const flag = pick(result, ["succeeded", "issuccess", "success"]);
            if (res.ok && (flag === undefined ? true : !!flag)) {
                const url = form.dataset.redirect;
                if (url) location.href = url; else location.reload();
                return;
            }

            const { general, fields } = parse(result);
            const leftover = applyFieldErrors(form, fields);
            const all = [...general, ...leftover];
            showSummary(form, all.length || Object.keys(fields).length ? all : ["Something went wrong."]);
        } catch {
            showSummary(form, ["Server error. Please try again."]);
        } finally {
            if (btn) btn.disabled = false;
        }
    });
})();