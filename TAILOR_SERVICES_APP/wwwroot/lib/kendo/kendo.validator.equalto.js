
//#region Kendo Validator EqualTo

function EqualToValidator(input) {
    if (input.filter("[data-val-equalto-other]").length) {
        let compareField = $(input).attr("data-val-equalto-other");
        compareField = compareField.substr(compareField.lastIndexOf(".") + 1);

        const status = input.val() == $(`#Input_${compareField}`).val();
        if (status) {
            if (_controlValidation.includes("compare-password")) _controlValidation.pop("compare-password");
        }
        else {
            if (!_controlValidation.includes("compare-password")) _controlValidation.push("compare-password");
        }
        return status;
    }
    return true;
}

//#endregion