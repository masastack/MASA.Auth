// user
export function onUserFileterAnimationEnd() {
    var filterDocument = document.getElementById('userFilter');
    if (filterDocument) {
        filterDocument.onanimationend = () => {
            if (filterDocument.className.includes('showAnimation') || filterDocument.className.includes('closeAnimation')) {
                filterDocument.className = filterDocument.className.replace('showAnimation', '').replace('closeAnimation', '');
            }
        };
    }
    var filterDocument2 = document.getElementById('userFilter2');
    if (filterDocument2) {
        filterDocument2.onanimationend = () => {
            if (filterDocument2.className.includes('showAnimation') || filterDocument2.className.includes('closeAnimation')) {
                filterDocument2.className = filterDocument2.className.replace('showAnimation', '').replace('closeAnimation', '');
            }
        };
    }
}