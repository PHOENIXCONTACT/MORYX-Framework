import { Pipe, PipeTransform } from '@angular/core';

/**
 * Inserts zero-width spaces after each occurrence of the separator (default: '.')
 * to allow the browser to break long strings like type names at those positions.
 */
@Pipe({
  name: 'wordBreak',
})
export class WordBreakPipe implements PipeTransform {
  transform(value: string | undefined | null, separator = '.'): string {
    if (!value) {
      return '';
    }
    return value.replaceAll(separator, separator + '\u200B');
  }
}
