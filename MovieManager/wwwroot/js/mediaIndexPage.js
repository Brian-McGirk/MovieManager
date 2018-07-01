function copyFunction() {
	var copyText = document.getElementById("showToCopyText");
	copyText.select();
	document.execCommand("copy");
}