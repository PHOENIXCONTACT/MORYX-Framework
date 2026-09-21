
function isDigit(value: string) : boolean {
  return value.length === 1 && value >= '0' && value <= '9';
}

export function blockNonDigitInput(event: InputEvent): void {
  const data = event.data;

  if (!data) {
    return;
  }

  if (Array.from(data).some(char => !isDigit(char))) {
    event.preventDefault();
  }
}

