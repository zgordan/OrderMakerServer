//Start
    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-field-delete'));

    document.querySelector('[mtd-data-delete]').addEventListener('click', () => {
        dialog.open();
    });
