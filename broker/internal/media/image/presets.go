package image

func DefaultVariantSpecs() []VariantSpec {
	return []VariantSpec{
		{
			Name:          "gallery",
			Width:         320,
			Height:        320,
			Fit:           FitContain,
			Format:        FormatAVIF,
			Quality:       72,
			StripMetadata: true,
			Background:    Color{R: 255, G: 255, B: 255},
		},
		{
			Name:          "spotlight",
			Width:         2000000,
			Height:        2000000,
			Fit:           FitContain,
			Format:        FormatWebP,
			Quality:       75,
			StripMetadata: true,
			Background:    Color{R: 255, G: 255, B: 255},
		},
	}
}
