const _controlValidation = [];
const _dataValidation = {};

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

//#region Kendo Validator Remote

function RemoteValidator(input, validator) {
    if (input.filter("[data-val-remote-url]").length) {

        function GetFieldSelector(parentForm, field) {
            const inputId = $(input).attr('id') || '';
            const prefix = inputId.substring(0, inputId.lastIndexOf('_') + 1) || 'Input_';
            const el = $(parentForm).find(`#${prefix}${field}`);
            return el.length ? el : $(parentForm).find(`[id$="_${field}"]`);
        }

        const requestAttr = $(input).attr("data-val-remote-requested");
        if (typeof requestAttr != typeof undefined) {
            return $(input).attr("data-val-remote-response") === 'true' ? true : false;
        }

        let thatControl;
        if ($(input).closest('.k-input.form-control').length > 0) thatControl = $(input).closest('.k-input.form-control');
        else thatControl = $(input).closest('.form-control');

        $(input).val($(input).val().trim());

        if ($(input).val().length > 0) {
            $(thatControl).removeAttr('data-status');
            $(thatControl).removeClass('validating valid invalid');

            const parentForm = $(validator.element);
            const additionalFields = $(input).attr("data-val-remote-additionalfields").split(",").map(item => item.replace('*.', ''));

            let parameter = '';
            let url = $(input).attr("data-val-remote-url");

            const tokenField = 'Token';
            if (additionalFields.includes(tokenField)) {
                const tokenEl = GetFieldSelector(parentForm, tokenField);
                const tokenValue = tokenEl.val() ?? null;
                const newAdditionalFields = additionalFields.filter(item => item !== tokenField);

                $.each(newAdditionalFields, function (index, field) {
                    parameter += parameter.length === 0 ? '?' : '&';
                    const fieldEl = GetFieldSelector(parentForm, field);

                    if ($(input).attr('name') === fieldEl.attr('name')) {
                        parameter += `${field}=`;
                        parameter += fieldEl.val() == undefined ? null : encodeURIComponent(EncryptData(fieldEl.val(), tokenValue));
                    } else {
                        parameter += `${field}=`;
                        parameter += fieldEl.val() ?? null;
                    }
                });
                parameter += `&token=${tokenValue}`;
            } else {
                $.each(additionalFields, function (index, field) {
                    parameter += parameter.length === 0 ? '?' : '&';
                    const fieldEl = GetFieldSelector(parentForm, field);
                    parameter += `${field}=`;
                    parameter += fieldEl.val() ?? null;
                });
            }

            if ($(input).attr("data-type") === 'phone-number') {
                let value = '';
                const fields = additionalFields.filter(name => !name.toLowerCase().includes("id"));
                fields.shift();
                $.each(fields, function (index, field) {
                    value = GetFieldSelector(parentForm, field).val();
                });
                value = `${value}-${$(input).val()}`;
                if ($(input).attr("data-snapshot")) if (value === $(input).attr('data-snapshot').trim()) return true;
            } else {
                if ($(input).attr("data-snapshot")) if ($(input).val().trim() === $(input).attr('data-snapshot').trim()) return true;
            }

            url = url + parameter;

            const thatInput = input;
            const thatValidator = validator;

            if (!_controlValidation.includes($(input).attr('name'))) _controlValidation.push($(input).attr('name'));
            _dataValidation[$(input).attr('name')] = undefined;

            $(input).attr("data-val-remote-requested", true);
            $(thatControl).addClass('validating');

            $.get(url, function (response) {
                $(input).attr("data-val-remote-response", response.isValid);

                if (!response.isValid) {
                    if (response.hasAlert) {
                        SystemAlert({ message: response.message });
                        $(input).attr("data-val-remote", '');
                    } else {
                        $(input).attr("data-val-remote", response.message);
                    }
                }

                thatValidator.validateInput(thatInput);
                _dataValidation[$(input).attr('name')] = response.isValid;

                const index = _controlValidation.indexOf($(input).attr('name'));
                if (index > -1) _controlValidation.splice(index, 1);

                setTimeout(() => thatInput.removeAttr("data-val-remote-requested"), 200);

                $(thatControl).removeClass('validating');
                switch (response.isValid) {
                    case true:
                        $(thatControl).addClass('valid');
                        break;
                    case false:
                        $(thatControl).attr('data-status', 'invalid');
                        $(thatControl).trigger('data-status');
                        $(thatControl).addClass('invalid');
                        break;
                }
            }).fail(() => {
                _dataValidation[$(input).attr('name')] = false;
                $(thatControl).removeClass('validating');
            });
        } else {
            $(thatControl).removeClass('validating valid invalid');
        }
    }
    return true;
}

//#endregion

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

//#region Kendo Validator Unique Name

function UniqueNameValidator(input, id) {
	const value = $(input).val();
	if (!value) return true;
	const arrayName = $(input).attr('data-val-uniquename-array');
	const fieldName = $(input).attr('data-val-uniquename-field');
	const arrayData = GetValidatorArrayData(arrayName);
	const duplicate = arrayData.some(x => String(x[fieldName]).trim().toLowerCase() === value.toLowerCase() && String(x.id) !== String(id));
	return !duplicate;
}

function GetValidatorArrayData(arrayName) {
	const hiddenInput = $(`#${arrayName}`);
	if (!hiddenInput.length) return [];
	try {
		return JSON.parse(hiddenInput.val()) || [];
	} catch {
		return [];
	}
}

//#endregion