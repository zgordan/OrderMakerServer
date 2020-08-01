﻿
document.querySelectorAll("[mtd-dialog-button]").forEach((item) => {

    const dialogId = item.getAttribute("mtd-dialog-button");
    const modal = document.getElementById("main-scrim");
    const dialog = document.getElementById(dialogId);
    const appContent = document.getElementById("drawer-frame-app-content");

    item.addEventListener("click", () => {
        dialog.classList.remove("mtd-dialog-hidden");
        modal.style.display = "block";
        document.body.style.overflow = "-moz-scrollbars-vertical";
        document.body.style.overflowY = "scroll";
        document.body.style.height = "100vh";
        appContent.classList.remove("drawer-frame-app-content");
        const tf = dialog.querySelector(".mdc-text-field--textarea");
        if (tf) {
            tf.focus();
        }

    });

    dialog.querySelectorAll('[mtd-dialog-cancel]').forEach((b) => {
        b.addEventListener("click", () => {
            dialog.classList.add("mtd-dialog-hidden");
            modal.style.display = "";
            document.body.style.overflow = "hidden";            
            document.body.style.overflowY = "";
            document.body.style.height = "";
            appContent.classList.add("drawer-frame-app-content");
        });
    });

})