
const clickBlockToogle = (el) => {
    const id = el.getAttribute('data');
    const content = document.getElementById(id);
    content.classList.toggle('mtd-desk-block-content--colapsed');
}



//Start

const selectOwner = new MTDSelectList("select-owner");

const selectDecision = new MTDSelectList("select-decision");
const commentStart = new MTDTextField("comment-start");

const selectDecisionConfirm = new MTDSelectList("select-decision-confirm");
const commentConfirm = new MTDTextField("comment-confirm");

const selectSatge= new MTDSelectList("select-stage");
const commentReturn = new MTDTextField("comment-return");

const selectDecisionReject = new MTDSelectList("select-decision-reject");
const commentReject = new MTDTextField("comment-reject");

const selectRecipient = new MTDSelectList("select-recipient");
const commentRequest = new MTDTextField("comment-request");
const commentConsidered = new MTDTextField("comment-considered");

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

const printButton = document.getElementById("print-button");

if (printButton) {

    document.getElementById("print-form").addEventListener("change", (e) => {
        let url = printButton.getAttribute("href");
        let re;
        if (e.target.checked) {
            re = "printForm=false"; url = url.replace(re, "printForm=true");
        } else { re = "printForm=true"; url = url.replace(re, "printForm=false");}

        printButton.setAttribute("href",url);
    });

    document.getElementById("print-info").addEventListener("change", (e) => {
        let url = printButton.getAttribute("href");
        let re;
        if (e.target.checked) {
            re = "printInfo=false"; url = url.replace(re, "printInfo=true");
        } else { re = "printInfo=true"; url = url.replace(re, "printInfo=false"); }

        printButton.setAttribute("href", url);
    });

    document.getElementById("print-approval").addEventListener("change", (e) => {
        let url = printButton.getAttribute("href");
        let re;
        if (e.target.checked) {
            re = "printApproval=false"; url = url.replace(re, "printApproval=true");
        } else { re = "printApproval=true"; url = url.replace(re, "printApproval=false"); }

        printButton.setAttribute("href", url);
    });
}

