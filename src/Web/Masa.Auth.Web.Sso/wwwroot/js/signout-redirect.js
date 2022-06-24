window.addEventListener("load", function () {
    var a = document.querySelector("a.PostLogoutRedirectUri");
    if (a) {
        console.log(a);
        window.location = a.href;
    }
});
