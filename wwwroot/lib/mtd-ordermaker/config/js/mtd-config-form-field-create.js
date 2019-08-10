(() => {

    const fieldType = new mdc.select.MDCSelect(document.getElementById("fieldType"));
    const fieldForm = new mdc.select.MDCSelect(document.getElementById("fieldForm"));
    const fieldWrapper = document.getElementById("fieldWrapper");
    
    fieldType.listen('MDCSelect:change', () => {    
       
        fieldWrapper.style.display = "none";
        if (fieldType.value === '11') {
            console.log(fieldType.value);
            fieldWrapper.style.display = "";
        }
    });

})();