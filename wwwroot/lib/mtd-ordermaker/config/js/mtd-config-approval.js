
//Start
    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-delete'));
    if (dialog) {
        document.querySelector('[mtd-data-delete]').addEventListener('click', () => {
            dialog.open();
        });
    }
