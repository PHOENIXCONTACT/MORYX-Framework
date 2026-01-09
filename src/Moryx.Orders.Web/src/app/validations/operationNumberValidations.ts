import { AbstractControl, ValidationErrors } from "@angular/forms";

export class OperationNumberValidations{
    

    static isOperationNumberNotValid(control:AbstractControl): ValidationErrors | null{

        //converts the value numeric and checks if the result is NaN or a number
        if(isNaN(Number(control.value)) || control.value.length!=4)
        return {isOperationNumberNotValid:true}

        return null;
    }

}