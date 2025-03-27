import { AbstractControl, ValidationErrors } from '@angular/forms';

export function passwordValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) return { required: true };

  const hasUpperCase = /[A-Z]/.test(value);
  const hasLowerCase = /[a-z]/.test(value);
  const hasNumber = /[0-9]/.test(value);
  const hasSpecialChar = /[!?*\.@#$%^&+=]/.test(value);
  const isValidLength = value.length >= 8;

  const valid = hasUpperCase && hasLowerCase && hasNumber && hasSpecialChar && isValidLength;

  return valid ? null : {
    passwordStrength: {
      hasUpperCase,
      hasLowerCase,
      hasNumber,
      hasSpecialChar,
      isValidLength
    }
  };
}
