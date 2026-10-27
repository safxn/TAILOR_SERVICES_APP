
//#region Kendo Validator Required

function RequiredValidator(input) {
    if (input.filter("[data-val-required]").length) {
        if ($(input).val().length === 0) {
            let thatControl;
            if ($(input).closest('.k-input.form-control').length > 0) thatControl = $(input).closest('.k-input.form-control');
            else thatControl = $(input).closest('.form-control');

            $(thatControl).removeClass('validating');
            $(thatControl).removeClass('valid');
            $(thatControl).removeClass('invalid');
            return false;
        }
        return true;
    }
    return true;
}

//#endregion