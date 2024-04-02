namespace Bam.Data.Objects;

public class ObjectDataFactory
{
    public ObjectDataFactory(IObjectEncoderDecoder encoderDecoder)
    {
        this.ObjectEncoderDecoder = encoderDecoder;
    }
    
    public  IObjectEncoderDecoder ObjectEncoderDecoder { get; init; }
    
    
}