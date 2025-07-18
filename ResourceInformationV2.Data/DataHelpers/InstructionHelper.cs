using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataHelpers {

    public class InstructionHelper(ResourceRepository resourceRepository) {
        private readonly ResourceRepository _resourceRepository = resourceRepository;

        public async Task<(List<Instruction>, bool)> GetInstructions(string code, CategoryType categoryType) {
            var instructions = (await _resourceRepository.ReadAsync(c => c.Instructions.Include(i => i.Source).Where(i => i.CategoryType == categoryType && i.Source.Code == code))).ToList();
            var source = instructions == null || instructions.Count == 0 ? await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == code)) : instructions[0].Source;
            bool isUsed = false;
            switch (categoryType) {
                case CategoryType.Faq:
                    isUsed = source?.UseFaqs ?? false;
                    break;

                case CategoryType.Note:
                    isUsed = source?.UseNotes ?? false;
                    break;

                case CategoryType.Person:
                    isUsed = source?.UsePeople ?? false;
                    break;

                case CategoryType.Publication:
                    isUsed = source?.UsePublications ?? false;
                    break;

                case CategoryType.Resource:
                    isUsed = source?.UseResources ?? false;
                    break;
            }
            return (instructions ?? [], isUsed);
        }

        public async Task<int> SaveInstructions(string text, int sourceId, CategoryType categoryType, FieldType fieldType) {
            var instruction = await _resourceRepository.ReadAsync(c => c.Instructions.FirstOrDefault(i => i.SourceId == sourceId && i.CategoryType == categoryType && i.FieldType == fieldType));
            if (instruction == null) {
                return await _resourceRepository.CreateAsync(new Instruction { CategoryType = categoryType, FieldType = fieldType, SourceId = sourceId, Text = text });
            } else {
                instruction.Text = text;
                return await _resourceRepository.UpdateAsync(instruction);
            }
        }
    }
}