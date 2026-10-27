
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