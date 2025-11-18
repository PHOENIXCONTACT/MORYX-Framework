import { Observable } from "rxjs";
import { OperationViewModel } from "src/app/models/operation-view-model";


export interface InterruptDialogData {
  operation: OperationViewModel;
  onSubmit: (guid: string, user: string | undefined) => Observable<void>;
}
