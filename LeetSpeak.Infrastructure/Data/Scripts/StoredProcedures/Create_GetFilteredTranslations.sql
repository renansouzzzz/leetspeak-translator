--DELIMITER $$

--CREATE PROCEDURE GetFilteredTranslations(
--    IN p_UserId VARCHAR(100),
--    IN p_SearchTerm VARCHAR(100),
--    IN p_StartDate DATETIME,
--    IN p_EndDate DATETIME
--)
--BEGIN
--    SELECT * FROM Translations
--    WHERE 
--        (p_UserId IS NULL OR UserId = p_UserId)  -- Adicionando filtro de UserId
--        AND (p_SearchTerm IS NULL OR 
--            OriginalText LIKE CONCAT('%', p_SearchTerm, '%') OR 
--            TranslatedText LIKE CONCAT('%', p_SearchTerm, '%'))
--        AND (p_StartDate IS NULL OR TranslationDate >= p_StartDate)
--        AND (p_EndDate IS NULL OR TranslationDate <= p_EndDate)
--    ORDER BY TranslationDate DESC;
--END $$

--DELIMITER ;

SELECT * FROM Translations
    WHERE 
        (p_UserId IS NULL OR UserId = p_UserId) AND
        (p_SearchTerm IS NULL OR 
         OriginalText LIKE CONCAT('%', p_SearchTerm, '%') OR 
         TranslatedText LIKE CONCAT('%', p_SearchTerm, '%'))
        AND (p_StartDate IS NULL OR TranslationDate >= p_StartDate)
        AND (p_EndDate IS NULL OR TranslationDate <= p_EndDate)
    ORDER BY TranslationDate DESC;