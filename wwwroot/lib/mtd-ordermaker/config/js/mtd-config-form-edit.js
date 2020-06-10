//Start
    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-form-delete'));

    document.querySelector('[mtd-data-delete]').addEventListener('click', () => {
        dialog.open();
    });