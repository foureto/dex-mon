export const copyToClipboard = (data: string) => {
  const listener = (e: ClipboardEvent) => {
    if (e?.clipboardData) {
      e.clipboardData.setData("text/plain", data);
      e.preventDefault();
    }
    document.removeEventListener("copy", listener);
  };
  document.addEventListener("copy", listener);
  document.execCommand("copy");
};
