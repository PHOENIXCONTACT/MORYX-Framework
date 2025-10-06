// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints
{
    internal static class Converter
    {
        internal static ICustomSerialization _serialization = new PossibleValuesSerialization(null, new EmptyValueProvider());

        internal static ActiveInstruction FromModel(InstructionModel instruction)
        {
            return new ActiveInstruction
            {
                Id = instruction.Id,
                Title = instruction.Sender,
                Results = instruction.Results.Select(r => new InstructionResult() { Key = r.Key, DisplayValue = r.DisplayValue}).ToArray(),
                Instructions = instruction.Items.Select(i =>
                    new VisualInstruction() { Type = i.ContentType, Content = i.Content, Preview = i.Preview }).ToArray()
            };
        }

        internal static InstructionModel ToModel(ActiveInstruction instruction)
        {
            InstructionResultModel[] results = Array.Empty<InstructionResultModel>();
            if (instruction.Results?.Count > 0)
                results = instruction.Results.Select(i => new InstructionResultModel { Key = i.Key, DisplayValue = i.DisplayValue }).ToArray();

            var model =  new InstructionModel
            {
                Id = instruction.Id,
                Sender = instruction.Title,
                Type = results.Length > 0 ? InstructionType.Execute : InstructionType.Display,
                Results = results,
                Inputs = instruction.Inputs == null ? null : EntryConvert.EncodeObject(instruction.Inputs, _serialization),
                Items = instruction.Instructions?.Select(i => new InstructionItemModel
                {
                    ContentType = i.Type,
                    Content = i.Content,
                    Preview = i.Preview
                }).ToArray()
            };

            return model;
        }

        private class EmptyValueProvider : IEmptyPropertyProvider
        {
            public void FillEmpty(object obj)
            {
                ValueProviderExecutor.Execute(obj, new ValueProviderExecutorSettings().AddDefaultValueProvider());
            }
        }
    }
}

