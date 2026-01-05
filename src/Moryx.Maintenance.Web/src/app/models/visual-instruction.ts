import { 
  MoryxControlSystemVisualInstructionsInstructionContentType, 
  MoryxControlSystemVisualInstructionsVisualInstruction } from "../api/models";

export interface VisualInstruction {
  type: MoryxControlSystemVisualInstructionsInstructionContentType;
  content: string;
  preview: string;
}


export function mapFromVisualInstruction(data:  MoryxControlSystemVisualInstructionsVisualInstruction): VisualInstruction{
  return <VisualInstruction> {
    type: data?.type,
    content: data?.content,
    preview: data?.preview
  }
}