namespace SampleGame;

@error
model Error {
  code: int32;
  message: string;
}

model Player {
  @visibility(Lifecycle.Read)
  id: int32;

  name: string;

  @visibility(Lifecycle.Read, Lifecycle.Update)
  level: int32;
}
