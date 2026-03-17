package image

func DefaultVariantSpecs() []VariantSpec {
	return []VariantSpec{
		{
			Name:          "gallery",
			Width:         320,
			Height:        320,
			Fit:           FitCover,
			Format:        FormatJPEG,
			Quality:       72,
			StripMetadata: true,
			Background:    Color{R: 255, G: 255, B: 255},
		},
	}
}
