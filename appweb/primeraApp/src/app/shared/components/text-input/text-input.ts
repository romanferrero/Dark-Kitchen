import { Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Icon, IconName } from '../icon/icon';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule, Icon],
  templateUrl: './text-input.html',
})
export class TextInput {
  control = input.required<FormControl>();
  label = input('');
  id = input('');
  type = input('text');
  placeholder = input('');
  autocomplete = input('');
  icon = input<IconName | null>(null);
  errors = input<Record<string, string>>({});

  activeError(): string | null {
    const c = this.control();
    if (!c.touched || c.valid) return null;
    const messages = this.errors();
    for (const key of Object.keys(messages)) {
      if (c.hasError(key)) return messages[key];
    }
    return null;
  }
}
