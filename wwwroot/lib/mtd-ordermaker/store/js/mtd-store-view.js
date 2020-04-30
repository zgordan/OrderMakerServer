const clickBlockToogle = (el) => {
    const id = el.getAttribute('data');
    const content = document.getElementById(id);
    content.classList.toggle('mtd-desk-block-content--colapsed');    
}

(() => {

    const dialogSetOwner = document.getElementById('dialog-setowner');
    if (dialogSetOwner) {
        const dialog = new mdc.dialog.MDCDialog(dialogSetOwner);
        document.getElementById('setowner').addEventListener('click', () => {
            dialog.open();
        });
    }

    const dialogReject = document.getElementById('dialog-reject');
    if (dialogReject) {
        const d = new mdc.dialog.MDCDialog(dialogReject);
        const rb = document.getElementById('reject-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }

    const dialogReturn = document.getElementById('dialog-return');
    if (dialogReturn) {
        const d = new mdc.dialog.MDCDialog(dialogReturn);
        const rb = document.getElementById('return-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }

    const dialogAccept = document.getElementById('dialog-accept');
    if (dialogAccept) {
        const d = new mdc.dialog.MDCDialog(dialogAccept);
        const rb = document.getElementById('accept-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }

    const dialogAcceptSign = document.getElementById('dialog-accept-sign');
    if (dialogAcceptSign) {
        const d = new mdc.dialog.MDCDialog(dialogAcceptSign);
        const rb = document.getElementById('accept-sign-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }

    const dialogReturnSign = document.getElementById('dialog-return-sign');
    if (dialogReturnSign) {
        const d = new mdc.dialog.MDCDialog(dialogReturnSign);
        const rb = document.getElementById('return-sign-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }


    const dialogRejectSign = document.getElementById('dialog-reject-sign');
    if (dialogRejectSign) {
        const d = new mdc.dialog.MDCDialog(dialogRejectSign);
        const rb = document.getElementById('reject-sign-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }

    const dialogRequest = document.getElementById('dialog-request');
    if (dialogRequest) {
        const d = new mdc.dialog.MDCDialog(dialogRequest);
        const rb = document.getElementById('request-button');
        if (rb) {
            rb.addEventListener('click', () => {
                d.open();
            });
        }
    }

    const checkComplete = document.getElementById("checkbox-complete");
    if (checkComplete) {
        checkComplete.addEventListener('change', () => {
            const stageSelector = document.getElementById("stage-selector");
            if (!checkComplete.checked) {
                stageSelector.classList.remove("mdc-select--disabled");
            } else {
                stageSelector.classList.add("mdc-select--disabled");
            }
        });
    }

    const eraser = document.getElementById("eraser");
    if (eraser) {
        const dialogDelete = new mdc.dialog.MDCDialog(document.getElementById('dialog-store-delete'));
        eraser.addEventListener('click', () => {
            dialogDelete.open();
        });
    }

    const newApproval = document.getElementById("new-approval");
    if (newApproval) {
        const dialogApproval = new mdc.dialog.MDCDialog(document.getElementById("dialog-new-approval"));
        newApproval.addEventListener('click', () => {
            dialogApproval.open();
        });
    }

    const startApproval = document.getElementById("button-start-approval");
    if (startApproval) {
        const dialogStart = new mdc.dialog.MDCDialog(document.getElementById("dialog-start"));
        startApproval.addEventListener('click', () => {
            dialogStart.open();
        });
    }

})();