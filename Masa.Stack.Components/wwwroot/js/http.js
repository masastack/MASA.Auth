export function upsertClaim(key, value) {
    const Http = new XMLHttpRequest();
    const url = 'Account/UpsertClaim?key=' + key + "&value=" + value;
    Http.open("put", url, false);
    Http.send();
}
