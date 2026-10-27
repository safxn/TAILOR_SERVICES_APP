
//#region Kendo Validator Compare

function CompareValidator(input) {
    if (input.filter("[data-val-compare-other]").length) {
        let compareField = $(input).attr("data-val-compare-other");
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